# Sensor Monitoring - Project Scaffold

Este repositório contém um scaffold em C# (.NET 8) com exemplos de APIs, regras de alerta, testes unitários e Docker Compose para executar o aplicativo e o banco de dados (TimescaleDB). Inclui também uma configuração simples para Kafka (para uso futuro) e MailHog para ver os e-mails de teste.

Siga os passos para rodar:

1. Tenha Docker e Docker Compose instalados.
2. Na raiz do projeto execute:
   ```
   docker-compose up --build
   ```
3. A API ficará disponível em http://localhost:5000 e o Swagger em http://localhost:5000/swagger

