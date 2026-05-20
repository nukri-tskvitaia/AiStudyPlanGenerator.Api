# AI Study Plan Generator API

A lightweight AI-powered ASP.NET Core 8 Web API that demonstrates structured AI planning workflows and reusable prompt skills using Anthropic Claude.

The project showcases how AI systems can first create a structured execution plan and then generate a final result using reusable instruction packages (skills).

---

# Features

- ASP.NET Core 8 Web API
- Anthropic Claude API integration
- Structured planning workflow
- Reusable prompt skills
- AI-generated study plans
- Multi-step AI orchestration
- Tool/function calling
- Strongly typed configuration using `IOptions`
- Request validation
- AI response validation
- Swagger/OpenAPI support
- Clean architecture structure
- CancellationToken support
- Safe JSON deserialization

---

# Architecture

```text
Client / Swagger
→ StudyPlanController
→ StudyPlanService
   → Planner AI call
      → Generates structured study outline
   → Skill selection
      → Beginner / Intermediate / Advanced skill
   → Final Generator AI call
      → Generates final detailed study plan
→ Validation
→ API response
```

---

# AI Concepts Demonstrated

| Concept | Used |
|---|---|
| Structured planning / Plan mode | Yes |
| Reusable prompt skills | Yes |
| Multi-step AI workflow | Yes |
| Tool/function calling | Yes |
| Backend validation | Yes |
| AI orchestration | Yes |

---

# Full Flow Explanation

## ① User sends learning goal

Example:

```json
{
  "goal": "Learn Docker basics",
  "days": 5,
  "level": "Beginner"
}
```

---

## ② Planner AI call creates structured outline

Claude first acts as a planning assistant.

It generates:

```json
{
  "topics": [
    "Docker Fundamentals",
    "Containers and Images",
    "Docker CLI Basics",
    "Building Docker Images",
    "Docker Compose"
  ]
}
```

This is the planning phase.

The final study plan is NOT generated yet.

---

## ③ API selects reusable skill

The API selects a reusable instruction package (skill) based on user level.

Example:

```text
BeginnerLearningSkill
```

Skill instructions influence:
- difficulty
- terminology
- pacing
- task complexity
- explanation depth

---

## ④ Final AI call generates detailed study plan

Claude receives:
- original goal
- structured outline
- reusable skill instructions

Then generates the final detailed study plan.

---

## ⑤ API validates final response

The API validates:
- correct number of days
- valid step ordering
- required fields
- non-empty topics/tasks

---

## ⑥ Final response returned

Example:

```json
{
  "summary": "A 5-day beginner-friendly Docker study plan.",
  "steps": [
    {
      "day": 1,
      "topic": "Docker Fundamentals",
      "task": "Install Docker and run hello-world."
    }
  ]
}
```

---

# Technologies Used

- ASP.NET Core 8 Web API
- C#
- Anthropic Claude API
- Swagger / OpenAPI
- System.Text.Json
- Options Pattern (`IOptions<T>`)

---

# Project Structure

```text
Controllers/
Models/
Options/
Services/
Skills/
Validation/
Exceptions/
```

---

# Reusable Skills

The project includes reusable prompt instruction packages.

Current skills:

- BeginnerLearningSkill
- IntermediateLearningSkill
- AdvancedLearningSkill

Each skill modifies:
- explanation complexity
- pacing
- task difficulty
- learning style

---

# Example Request

```json
{
  "goal": "Learn Docker basics",
  "days": 5,
  "level": "Beginner"
}
```

---

# Example Response

```json
{
  "summary": "A 5-day beginner-friendly study plan to learn Docker basics.",
  "steps": [
    {
      "day": 1,
      "topic": "Docker Fundamentals and Installation",
      "task": "Install Docker and run hello-world."
    },
    {
      "day": 2,
      "topic": "Containers and Images",
      "task": "Learn the difference between containers and images."
    }
  ]
}
```

---

# Configuration

Store configuration securely using User Secrets or environment variables.

Example configuration:

```json
{
  "Claude": {
    "ApiKey": "your_claude_api_key",
    "Version": "2023-06-01",
    "Model": "claude-haiku-4-5-20251001",
    "MaxTokens": 1200,
    "Temperature": 0.2,
    "MessagesEndpointUrl": "https://api.anthropic.com/v1/messages"
  }
}
```

---

# Running the Project

## Clone repository

```bash
git clone <your_repo_url>
```

---

## Navigate to project

```bash
cd AiStudyPlanGenerator.Api
```

---

## Restore packages

```bash
dotnet restore
```

---

## Run the API

```bash
dotnet run
```

Swagger UI will open automatically.

---

# API Endpoint

## Generate Study Plan

```http
POST /api/study-plans/generate
```

---

# Validation

The application validates:

- Request model input
- Required AI response fields
- Correct number of study days
- Proper day ordering
- Non-empty topics
- Non-empty tasks
- JSON deserialization integrity

---

# Notes

This project demonstrates how AI systems can use structured planning before execution instead of directly generating final output immediately.

The architecture intentionally separates:

```text
Planning
↓
Skill selection
↓
Final generation
```

This produces:
- more controllable AI output
- reusable AI behavior
- cleaner orchestration patterns
- more consistent study plans

The project remains intentionally lightweight while demonstrating modern AI backend architecture concepts.