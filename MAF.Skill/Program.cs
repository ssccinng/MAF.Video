// Copyright (c) Microsoft. All rights reserved.

// This sample demonstrates how to define Agent Skills entirely in code using AgentInlineSkill.
// No SKILL.md files are needed — skills, resources, and scripts are all defined programmatically.
//
// Three approaches are shown using a unit-converter skill:
// 1. Static resources — inline content provided via AddResource
// 2. Dynamic resources — computed at runtime via a factory delegate
// 3. Code scripts — executable delegates the agent can invoke directly

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text.Json;

#pragma warning disable MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.



// --- Configuration ---
string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT 未设置。");
string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.4-mini";

// --- Build the code-defined skill ---
var unitConverterSkill = new AgentInlineSkill(
    name: "unit-converter",
    description: "使用乘法因子在常见单位之间进行转换。用于请求转换英里、千米、磅或公斤时。",
    instructions: """
        当用户请求在单位之间进行转换时使用此技能。

        1. 查看 conversion-table 资源以查找所请求转换的因子。
        2. 检查 conversion-policy 资源以获取舍入和格式化规则。
        3. 使用 convert 脚本，传入表格中的数值和因子。
        """)
    // 1. Static Resource: conversion tables
    .AddResource(
        "conversion-table",
        """
        # 转换表

        公式：**result = value × factor**

        | 转换自      | 转换到      | 因子     |
        |-------------|-------------|----------|
        | miles       | kilometers  | 1.60934  |
        | kilometers  | miles       | 0.621371 |
        | pounds      | kilograms   | 0.453592 |
        | kilograms   | pounds      | 2.20462  |
        """)
    // 2. Dynamic Resource: conversion policy (computed at runtime)
    .AddResource("conversion-policy", () =>
    {
        const int Precision = 4;
        return $"""
            # 转换策略

            **小数位数：** {Precision}
            **格式：** 始终同时显示原始值和转换后的值以及单位
            **生成时间：** {DateTime.UtcNow:O}
            """;
    })
    // 3. Code Script: convert
    .AddScript("convert", (double value, double factor) =>
    {
        double result = Math.Round(value * factor, 4);
        return JsonSerializer.Serialize(new { value, factor, result });
    });

// --- Skills Provider ---
var skillsProvider = new AgentSkillsProvider(unitConverterSkill);

// --- Agent Setup ---
AIAgent agent = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "单位转换代理",
        ChatOptions = new()
        {
            Instructions = "你是一个可以进行单位转换的助理。",
        },
        AIContextProviders = [skillsProvider],
    });

// --- Example: Unit conversion ---
Console.WriteLine("使用代码定义的技能进行单位转换");
Console.WriteLine(new string('-', 60));

AgentResponse response = await agent.RunAsync(
    "马拉松（26.2 英里）是多少公里？75 公斤等于多少磅？");

Console.WriteLine($"代理：{response.Text}");

#pragma warning restore MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
