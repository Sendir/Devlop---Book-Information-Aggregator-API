# Devlop - Book Information Aggregator API

A C# ASP.NET Core Web API that allows you to manage a local book collection and fetch book information from the Open Library API.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Running the API](#running-the-api)
- [Testing](#testing)
- [Endpoints](#endpoints)

---

## Project Overview

This API serves two main purposes:

1. Manage a **local book collection** stored in `books.json`.
2. Search and retrieve **book information from the Open Library API**.

It is built with **.NET 9**, **C#**, and follows RESTful principles.

---

## Features

- Get all local books
- Add a new local book
- Search books using Open Library API
- Fetch detailed book information by Open Library work key

---

## Prerequisites

Make sure you have the following installed:

- .NET 9 SDK
- Prefered IDE
- Git (optional, for version control)

---

## Setup

1. Clone the repository:

```bash
git clone https://github.com/Sendir/Devlop---Book-Information-Aggregator-API.git
cd Devlop---Book-Information-Aggregator-API

2. Restore NuGet packages: 
dotnet restore

3. Build the project:

dotnet build
```

## Running the API
```bash
dotnet run --project bookapi
```
## Testing
```bash
dotnet test ./bookapi.Tests/bookapi.Tests.csproj
```
## Endpoints

All endpoints are prefixed with `/books`.

---

### Get All Local Books

**GET** `/books`

Retrieves all books stored locally.

**Responses**

| Status | Description | Content |
|--------|-------------|---------|
| 200    | OK          | `application/json` (array of `BookDto`) |

**BookDto Schema**

```json
{
  "id": 1,
  "title": "string",
  "author": "string",
  "description": "string",
  "published_year": 2024
}
```
---
### Add Local Book
**POST** `/books`

Adds a book to the local collection.

**Request Body**

**CreateBookDto Schema**

```json
{
  "id": 1,
  "title": "string",
  "author": "string",
  "description": "string",
  "published_year": 2024
}
```

**Responses**

| Status | Description | Content |
|--------|-------------|---------|
| 200    | OK          | `application/json` (`BookDto`) |

**BookDto Schema**

```json
{
  "id": 1,
  "title": "string",
  "author": "string",
  "description": "string",
  "published_year": 2024
}
```

**Notes**

- If a book with the same title and author already exists, the API will return a `409 Conflict`.
- `description` is optional but cannot exceed 500 characters.
- `published_year` must be a positive integer.
---

### Search Books (OpenLibrary)

**GET** `/books/search`

Searches books from the OpenLibrary API.

**Query Parameters**

| Name       | Type    | Description                          | Default |
|------------|---------|--------------------------------------|---------|
| query      | string  | The search query (required)           | -       |
| PageNumber | integer | Page number for pagination (optional) | 1       |
| PageSize   | integer | Number of results per page (optional) | 5       |

**Responses**

| Status | Description | Content |
|--------|-------------|---------|
| 200    | OK          | `application/json` (OpenLibrary search results) |

**Notes**

- `query` parameter is required. If omitted, API will return a `400 Bad Request`.
- Pagination parameters are optional; defaults are `PageNumber=1` and `PageSize=5`.
---
### Get Book Details by ID (OpenLibrary)

**GET** `/books/{id}`

Retrieves detailed information about a book from OpenLibrary using its work ID.

**Path Parameters**

| Name | Type   | Description          | Required |
|------|--------|--------------------|----------|
| id   | string | The work ID of book | Yes      |

**Responses**

| Status | Description | Content |
|--------|-------------|---------|
| 200    | OK          | `application/json` (`OpenLibraryDetailsDto`) |

**OpenLibraryDetailsDto Schema**

```json
{
  "work_key": "OL12345W",
  "author_key": ["OL12345A"],
  "title": "Book Title",
  "first_publish_date": "2023",
  "subjects": ["Subject1", "Subject2"],
  "description": "Book description"
}
```
**Notes**

- Returns `200 OK` if the book is found.
- The work_key in the response corresponds to the requested id.
---