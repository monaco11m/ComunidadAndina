# Order Management System

Sistema de gestión de pedidos con envío de confirmaciones por correo y procesamiento asíncrono mediante RabbitMQ.

Este proyecto fue desarrollado siguiendo buenas prácticas de arquitectura (Clean Architecture), principios SOLID, inyección de dependencias, validaciones con FluentValidation, mensajería con RabbitMQ y entrega de emails mediante MailHog.

---

## Tecnologías Utilizadas

- .NET 8 (Web API)
- Entity Framework Core
- SQL Server
- RabbitMQ (con Management UI)
- MailHog (SMTP + Web UI)
- Serilog (logging estructurado)
- Docker & Docker Compose

---


## Ejecución del Proyecto con Docker

1. Abrir una terminal en la carpeta raíz del proyecto:

```
cd OrderManagement
```

2. Levantar todos los servicios:

```
docker compose up --build -d
```

3. Confirmar que los contenedores están corriendo:

```
docker ps
```

---

## Servicios y Puertos

| Servicio | URL | Descripción |
|---------|-----|-------------|
| API REST (.NET) | http://localhost:5000/swagger | Documentación y pruebas |
| SQL Server | localhost:1433 | Base de datos |
| RabbitMQ UI | http://localhost:15672 (guest / guest) | Consola de monitoreo |
| MailHog UI | http://localhost:8025 | Ver correos enviados |

---

## Probando el Flujo Completo

1. Crear un pedido (POST `/api/orders`)
2. Se descuenta stock y se guarda en DB
3. Se emite evento a RabbitMQ
4. El consumidor procesa el evento
5. MailHog recibe el correo de confirmación

Puedes ver los correos en:  
 http://localhost:8025

---

## Migraciones y Seed (solo si corres la API fuera de Docker)

```
cd OrderManagement.API
dotnet ef database update
```

---

## Autor

Elmer — Backend Developer (.NET)

---

