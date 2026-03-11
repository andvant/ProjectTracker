# Project Tracker

Веб-приложение для управления проектами и отслеживания задач

## Возможности

- **Управление проектами**: создание и редактирование проектов, добавление участников, вложений, смена владельца проекта
- **Отслеживание задач**: создание и редактирование задач, назначение исполнителей из числа участников проекта, добавление наблюдателей, вложений, комментариев
- **Уведомления**: просмотр уведомлений пользователя о событиях в системе с отслеживанием статуса прочтения
- **Аутентификация и авторизация**: регистрация, авторизация и просмотр списка пользователей, назначение ролей (добавление в группы)

### Уведомления
Пользователям приходят уведомления при следующих событиях:
- Пользователь был добавлен как участник проекта
- Пользователь был назначен новым владельцем проекта
- На пользователя была назначена задача
- Задача, которую отслеживает пользователь, была отредактирована

### Пользователи, роли
Регистрация доступна в окне входа Keycloak (при нажатии кнопки Sign in в веб-приложении либо Authorize в Swagger UI)

Действия пользователей проходят авторизацию (имеет ли пользователь нужную роль, является ли участником проекта, и т. д.)

Предварительно созданные пользователи (пароль 123):
- admin (роль Admin, доступны все действия)
- mgr (роль Project Manager, может создавать и просматривать все проекты)
- user (обычный пользователь, может просматривать только проекты в которые добавлен как участник)

## Технологии

### Бэкенд
- **Язык и фреймворк**: C#, ASP.NET Core 10 (Minimal API)
- **Архитектура**: Clean Architecture, CQRS, DDD, Domain Events
- **База данных, ORM**: PostgreSQL + EF Core
- **Аутентификация, авторизация**: Keycloak (OAuth2, OIDC)
- **Хранилище файлов**: RustFS (S3-compatible)
- **Кэширование**: Redis + FusionCache
- **Маппинг объектов**: Mapperly
- **Валидация**: FluentValidation, также доменные сущности валидируют своё состояние
- **Логирование, мониторинг**: OpenTelemetry
- **Документация API**: OpenAPI (Swagger)
- **Тестирование**: xUnit, Shouldly, AutoFixture

### Слои бэкенда
- **Domain**: Entities, Value Objects, Domain Events, Domain Exceptions
- **Application**: Use cases (Command/Query/Event handlers), Interfaces, Application Exceptions
- **Infrastructure**: Integrations with database, caching, object storage, Keycloak admin API
- **Api**: Endpoints, Exception handlers, entry point

### Фронтенд
- **Язык и фреймворк**: TypeScript, Vue 3
- **Сборщик**: Vite
- **Управление состоянием**: Pinia
- **Аутентификация**: oidc-client-ts

### Инфраструктура
- **Контейнеризация**: Docker
- **Оркестрация**: Docker Compose, Aspire
- **CI/CD**: GitHub Actions

### CI/CD workflows
- **build.yaml**: сборка, тестирование и загрузка артефактов (api, web)
- **push.yaml**: сборка docker образов (api, web) и их загрузка в Docker Hub

## Локальная разработка и запуск

### Предварительные требования:
- .NET 10+ SDK
- Node.js 24+
- Docker и Docker Compose либо Aspire (13+) для оркестрации сервисов

### Запуск сервисов по отдельности:

#### API:
```sh
dotnet run --project backend/src/Api/Api.csproj
```

#### Web:
```sh
cd frontend
npm install
npm run dev
```

#### Адреса запущенных сервисов:
- API: http://localhost:5050/swagger
- Web: http://localhost:5150

### Запуск всех сервисов с помощью Docker Compose:
```sh
docker compose up
```

Чтобы подтянуть образы сервисов api и web из Docker Hub вместо локальной сборки:
```sh
docker compose -f compose.yaml -f compose.prod.yaml up
```

### Запуск всех сервисов с помощью Aspire:
```sh
aspire run
```

### Экспорт Keycloak realm (в папку /container-init/keycloak/):
```sh
docker exec -it <keycloak_container_id> /opt/keycloak/bin/kc.sh export --realm project-tracker --dir /tmp/export --users realm_file
docker cp <keycloak_container_id>:/tmp/export/project-tracker-realm.json ./container-init/keycloak/project-tracker-realm.json
```

Импорт происходит автоматически при создании контейнеров (с помощью docker compose up или aspire run)
