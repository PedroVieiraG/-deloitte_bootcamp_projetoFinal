# API - Monitoramento de Equipamentos Pesados

Uma API desenvolvida em **.NET 10** para gerenciar o ciclo de vida e a manutenção de equipamentos de minas (caminhões, escavadeiras, perfuratrizes, etc.).

## Tecnologias Utilizadas
* **C# / .NET 10**
* **Entity Framework Core** 
* **PostgreSQL**
* **Docker**
* **Insomnia** (para testes da API)

## Funcionalidades
* **CRUD Completo:** Criação, listagem, detalhamento, atualização e remoção de equipamentos.
* **Validações:** Código único por equipamento, bloqueio de horímetro negativo na criação e na atualização.
* **Regras de Exclusão:** Um equipamento não pode ser deletado se possuir manutenções associadas com status "Concluída".
* **Endpoints de Negócio:**
  * Atualização isolada de Horímetro (`PATCH`).
  * Mudança de Status Operacional (`PATCH`).
  * Dashboard Estatístico (`GET`) mostrando o panorama da frota.
  * Histórico de Manutenções por Equipamento (`GET`).

## Como Executar o Projeto:

### Pré-requisitos
* [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e rodando.
* [.NET 10 SDK](https://dotnet.microsoft.com/download) 

### Passo a Passo (Via Docker Compose)
1. Clone este repositório:
   ```bash
   git clone https://github.com/PedroVieiraG/-deloitte_bootcamp_projetoFinal.git
2. Depois:
    ```bash
   docker-compose up -d --build

   
