# CliChat Server

CliChat Server is the backend for the **Mi(j)Chat** CLI chat application. It provides Web APIs and a SignalR hub to enable secure, real-time, end-to-end encrypted communication between users.

The codebase is clear and well-commented.

More details about the project can be found in the [CliChatClient](https://github.com/Neyronbite/CliChatClient) repository.

---

## Overview

- Written in .NET 8
- Exposes port **443**
- 200 mbs are more than enough for DB

---

## Environment Variables

| Variable         | Description                                                   |
|------------------|---------------------------------------------------------------|
| `JWT_SECRET`     | Secret key for signing JWT tokens (**required**)              |
| `CERT_PASSWORD`  | Password for the PFX certificate (**required**)               |
| `DB_VOL`         | Path to database storage volume (default: `/var/michat-db`)   |
| `STATIC_PATH`    | Path for static files (default: `/var/michat/www-root`)       |
| `CERT_PATH`      | Full path to PFX certificate (default: `/var/michat-cert/michat.pfx`) |

Only `JWT_SECRET` and `CERT_PASSWORD` are required — the rest have sensible defaults.

---

## For Docker

## Volumes

You need to mount two directories when running the container:

1. **Database Volume**
   - Default internal path: `/var/michat-db`
   - Example: `michat-db:/var/michat-db`

2. **Certificate Volume**
   - Default internal path: `/var/michat-cert`
   - Example: `/your/pfx/path:/var/michat-cert`

## Quick docker run instructions

```bash
docker run -v /var/cert/michat:/var/michat-cert -v michat-db:/var/michat-db -e JWT_SECRET=secret -e CERT_PASSWORD=password -p 443:443 neyronbite/michat:1.1
```

---

## Compose file example

```yml
services:
  michat:
    image: neyronbite/michat:1.1
    container_name: michat
    restart: unless-stopped
    volumes:
      - "/path/to/your/cert/folder:/var/michat-cert"
      - "michat-db:/var/michat-db"
    ports:
      - "443:443"
    environment:
      - "JWT_SECRET=YourJwtSecret"
      - "CERT_PASSWORD=YourCertPassword"

volumes:
  michat-db:
```