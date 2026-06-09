# Personal Blog — Backend API Reference

HTTP reference for the ASP.NET Core API (`PersonalBlog.Api`), with copy-paste `curl`
examples for every endpoint, including auth.

> Source of truth: `PersonalBlog.Api/Controllers/*`. If a route changes there,
> update this file.

---

## Base URLs

| Profile | URL |
|---------|-----|
| HTTP  | `http://localhost:5122` |
| HTTPS | `https://localhost:7052` |

The examples below use the HTTP profile. For HTTPS with the dev certificate, add
`-k` to each `curl` (skip TLS verification) and swap the base URL.

A convenience variable used throughout:

```bash
BASE=http://localhost:5122
```

### Route prefixes — read this first

Both controllers declare `[Route("api")]`, **but**:

- **Auth / account** routes use *relative* templates, so they sit under `/api`
  (e.g. `/api/auth/login`).
- **Blog** and **draft** routes use *leading-slash* templates (`"/blogs"`,
  `"/drafts/..."`), which **override** the controller prefix — so they live at
  the **root**, with **no `/api`** (e.g. `/blogs`, not `/api/blogs`).

This mirrors how the Next.js BFF calls them.

---

## Authentication model

- Login issues a **JWT access token** + a **refresh token**, returned as
  **HttpOnly cookies** (`access_token`, `refresh_token`; `Secure`, `SameSite=None`).
- The API validates the JWT from the **`Authorization: Bearer <jwt>` header only**
  — there is no cookie→header bridge in .NET. In production that translation is
  done by the Next.js BFF, which reads the HttpOnly cookie server-side and
  forwards it as a Bearer header.
- **For direct `curl` calls** you therefore need to pull the token out of the
  cookie jar and send it as a Bearer header. The auth guards are:
  - `[AllowAnonymous]` — open.
  - `[Authorize]` — any valid JWT.
  - `[IdentityFilter]` — valid JWT **and** the token's `sub` claim must equal the
    `{id}` route param (you can only act on your own identity).

### Getting a token for protected endpoints

```bash
# 1. Log in, capturing Set-Cookie into a cookie jar
curl -i -c cookies.txt -X POST "$BASE/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"userName":"jdoe","password":"P@ssw0rd!"}'

# 2. Extract the JWT from the cookie jar (Netscape cookie format: value is column 7)
TOKEN=$(awk '/access_token/ {print $7}' cookies.txt)

# 3. Use it on any protected endpoint
curl -X GET "$BASE/api/auth/me" -H "Authorization: Bearer $TOKEN"
```

Most protected routes also need a **user GUID** in the path. Grab it from the
login/`/api/auth/me` response and export it:

```bash
USER_ID=00000000-0000-0000-0000-000000000000
```

---

## Endpoint summary

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/login` | Anonymous | Authenticate; sets token cookies |
| POST | `/api/auth/register` | Anonymous | Register a new user |
| POST | `/api/auth/logout` | Open | Sign out (clears Identity session) |
| POST | `/api/auth/refresh` | Anonymous + refresh cookie | Rotate tokens |
| GET | `/api/auth/me` | Bearer | Current user from JWT |
| GET | `/api/auth/user/{id}` | IdentityFilter | User info for `{id}` |
| GET | `/api/auth/identity/{id}` | IdentityFilter | Identity info for `{id}` |
| PUT | `/api/account/edit/{id}` | Bearer (self) | Edit account details |
| GET | `/blogs` | Anonymous | List all published posts |
| GET | `/blogs/{id}` | Anonymous | Get one post |
| POST | `/blogs/create/{id}` | Bearer | Create a post (`{id}` = author GUID) |
| PUT | `/blogs/{postId}/users/{id}` | Bearer | Update a post |
| DELETE | `/blogs/{postId}/users/{id}/delete` | Bearer | Delete a post |
| POST | `/blogs/sharelink/send-email` | Anonymous | Email a share link |
| POST | `/drafts/users/{userId}/create` | Bearer | Create a draft |
| PUT | `/drafts/{draftId}/users/{userId}/edit` | Bearer | Update a draft |
| GET | `/drafts/users/{userId}` | Bearer | List a user's drafts |
| GET | `/drafts/{draftId}/users/{userId}` | Bearer | Get one draft |
| DELETE | `/drafts/{draftId}/users/{userId}/delete` | Bearer | Delete a draft |
| POST | `/drafts/{draftId}/users/{userId}/publish` | Bearer | Publish a draft as a post |

---

## Auth & account

### POST `/api/auth/login`
Authenticate. On success, sets `access_token` + `refresh_token` HttpOnly cookies
and returns the user.

```bash
curl -i -c cookies.txt -X POST "$BASE/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "jdoe",
    "password": "P@ssw0rd!"
  }'
```

**200** →
```json
{ "user": { "id": "…", "userName": "jdoe", "email": "jdoe@example.com" } }
```
**401** → `"Failed to authenticate credentials."`

---

### POST `/api/auth/register`
Create a new user. `confirmPassword` is optional.

```bash
curl -X POST "$BASE/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "jdoe",
    "password": "P@ssw0rd!",
    "confirmPassword": "P@ssw0rd!",
    "email": "jdoe@example.com",
    "firstName": "Jane",
    "lastName": "Doe"
  }'
```

**200** → `"User registered successfully"`

---

### POST `/api/auth/logout`
Clears the ASP.NET Identity sign-in. (No `[Authorize]` attribute — callable
without a token.)

```bash
curl -X POST "$BASE/api/auth/logout"
```

**200** → `"User logged out successfully"`

---

### POST `/api/auth/refresh`
Exchanges a valid `refresh_token` cookie for a new access/refresh pair (the old
refresh token is rotated/invalidated). Send the cookie jar and let curl update it.

```bash
curl -i -b cookies.txt -c cookies.txt -X POST "$BASE/api/auth/refresh"
```

**200** → `{ "user": { "id": "…", "userName": "…", "email": "…" } }`
(plus refreshed cookies)
**401** → `"Missing refresh token."` (or expired/used/revoked)

---

### GET `/api/auth/me`
Returns the user resolved from the JWT `sub` claim.

```bash
curl -X GET "$BASE/api/auth/me" -H "Authorization: Bearer $TOKEN"
```

**200** → `{ "user": { "id": "…", "userName": "…", "email": "…" } }`
**401** → no/invalid token.

---

### GET `/api/auth/user/{id}`
User info for `{id}`. `[IdentityFilter]`: `{id}` must match your token's `sub`.

```bash
curl -X GET "$BASE/api/auth/user/$USER_ID" -H "Authorization: Bearer $TOKEN"
```

---

### GET `/api/auth/identity/{id}`
Identity info for `{id}`. `[IdentityFilter]`: `{id}` must match your token's `sub`.

```bash
curl -X GET "$BASE/api/auth/identity/$USER_ID" -H "Authorization: Bearer $TOKEN"
```

---

### PUT `/api/account/edit/{id}`
Edit account details. `[Authorize]` and `{id}` must equal your own user GUID.

```bash
curl -X PUT "$BASE/api/account/edit/$USER_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "jdoe",
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane.doe@example.com"
  }'
```

**200** → `{ "isUpdated": true, "userGuid": "…" }`

---

## Blogs

### GET `/blogs`
List all published posts.

```bash
curl -X GET "$BASE/blogs"
```

**200** →
```json
{
  "blogs": [
    {
      "id": "…", "title": "…", "content": "…", "preview": "…",
      "datePosted": "2026-06-01T12:00:00Z", "author": "Jane Doe", "userId": "…"
    }
  ]
}
```

---

### GET `/blogs/{id}`
Get a single post.

```bash
curl -X GET "$BASE/blogs/$POST_ID"
```

**200** → `{ "blog": { …same shape as above… } }`
**404** → not found.

---

### POST `/blogs/create/{id}`
Create a post. `{id}` is the **author's user GUID**. `[Authorize]`.

```bash
curl -X POST "$BASE/blogs/create/$USER_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Hello World",
    "content": "Full body of the post…",
    "preview": "Short summary shown in the list."
  }'
```

**200** → `{ "isSaved": true, "postGuid": "…" }`

---

### PUT `/blogs/{postId}/users/{id}`
Update a post. `{postId}` = post GUID, `{id}` = author GUID. `[Authorize]`.
(The body's `Id` is set from `{postId}` server-side.)

```bash
curl -X PUT "$BASE/blogs/$POST_ID/users/$USER_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Hello World (edited)",
    "content": "Updated body…",
    "preview": "Updated summary."
  }'
```

**200** → `{ "isSaved": true, "postGuid": "…" }`
**404** → post not found / not owned.

---

### DELETE `/blogs/{postId}/users/{id}/delete`
Delete a post. `[Authorize]`.

```bash
curl -X DELETE "$BASE/blogs/$POST_ID/users/$USER_ID/delete" \
  -H "Authorization: Bearer $TOKEN"
```

**200** → `{ "isDeleted": true, "postGuid": "…" }`
**404** → post not found / not owned.

---

### POST `/blogs/sharelink/send-email`
Email a blog share link. Intentionally **anonymous** — no token required.
`identityUserId` is optional (null for anonymous senders).

```bash
curl -X POST "$BASE/blogs/sharelink/send-email" \
  -H "Content-Type: application/json" \
  -d '{
    "postId": "00000000-0000-0000-0000-000000000000",
    "recipientEmail": "friend@example.com",
    "identityUserId": null,
    "blogShareLink": "http://localhost:3000/blogs/00000000-0000-0000-0000-000000000000"
  }'
```

**200** → `{ "isTriggered": true, "eventGuid": "…" }`

---

## Drafts

All draft endpoints require `[Authorize]` (Bearer token) and a `{userId}` GUID
in the path.

### POST `/drafts/users/{userId}/create`

```bash
curl -X POST "$BASE/drafts/users/$USER_ID/create" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Untitled draft",
    "content": "Work in progress…",
    "preview": "Draft summary."
  }'
```

**200** → save result (`{ "isSaved": true, … }`)
**400** → `"Failed to create draft"`

---

### PUT `/drafts/{draftId}/users/{userId}/edit`

```bash
curl -X PUT "$BASE/drafts/$DRAFT_ID/users/$USER_ID/edit" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Untitled draft (edited)",
    "content": "More progress…",
    "preview": "Updated draft summary."
  }'
```

**200** → save result
**400** → `"Failed to update draft"`

---

### GET `/drafts/users/{userId}`
List all drafts for a user.

```bash
curl -X GET "$BASE/drafts/users/$USER_ID" -H "Authorization: Bearer $TOKEN"
```

---

### GET `/drafts/{draftId}/users/{userId}`
Get a single draft.

```bash
curl -X GET "$BASE/drafts/$DRAFT_ID/users/$USER_ID" \
  -H "Authorization: Bearer $TOKEN"
```

---

### DELETE `/drafts/{draftId}/users/{userId}/delete`

```bash
curl -X DELETE "$BASE/drafts/$DRAFT_ID/users/$USER_ID/delete" \
  -H "Authorization: Bearer $TOKEN"
```

**200** → `{ "isDeleted": true, … }`
**404** → draft not found / not owned.

---

### POST `/drafts/{draftId}/users/{userId}/publish`
Publish a draft as a post. Body carries the (possibly edited) final content.

```bash
curl -X POST "$BASE/drafts/$DRAFT_ID/users/$USER_ID/publish" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Hello World",
    "content": "Final published body…",
    "preview": "Published summary."
  }'
```

**200** → save result (`{ "isSaved": true, … }`)
**400** → `"Failed to publish blog post"`

---

## Error responses

Unhandled domain exceptions are normalized by `ExceptionHandlingMiddleware` into:

```json
{ "statusCode": 400, "message": "…" }
```

| Exception | Status |
|-----------|--------|
| `BadRequestException` | 400 |
| `FailedAuthenticationException` | 401 |
| `UnauthorizedException` | 401 |
| `UserNotFoundException` | 404 |
| (anything else) | 500 — `"Internal Server Error."` |

---

## Interactive docs

With the API running in Development, Swagger UI is available at:

```
http://localhost:5122/swagger
```
