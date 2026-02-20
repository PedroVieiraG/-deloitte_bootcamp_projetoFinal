# Desafio Final Bootcamp Deloitte

API desenvolvida em ASP.NET Core (.NET 10) para gerenciamento de equipamentos pesados, permitindo cadastro, consulta, atualização, remoção e operações específicas como avanço de status, atualização de horímetro e localização.

# Tecnologias Utilizadas

- .NET 10 
- Entity Framework Core
- SQL Server
- Docker
- Insomnia (arquivo exportado incluso no repositório)

## Funcionalidades da API

- CRUD completo de equipamentos
- Consulta por ID
- Consulta por tipo
- Consulta de status
- Avançar status (fluxo circular)
- Atualizar horímetro
- Atualizar localização

## Fluxo de Status Operacional

O status segue o seguinte fluxo:
```bash
Operacional → ForaDeServico → EmManutencao → Operacional
```

## Endpoints 

Base URL:
```bash
http://localhost:8080/api/equipamentos
```
**GET /api/equipamentos**
Lista todos os equipamentos

**GET /api/equipamentos/{id}**
Retorna equipamento por ID.

**GET /api/equipamentos/{id}/status**
Retorna apenas o status operacional.

**GET /api/equipamentos/{tipo}/tipo**
Busca equipamentos por tipo.

**POST /api/equipamentos**
Cria um novo equipamento.

**PUT /api/equipamentos/{id}**
Atualiza equipamento completo.

**PATCH /api/equipamentos/{id}/avancar-status**
Avança o status operacional.

**PATCH /api/equipamentos/{id}/atualizar-horimetro**
Atualiza apenas o horímetro.

**PATCH /api/equipamentos/{id}/atualizar-localizacao**
Atualiza apenas a localização.

**DELETE /api/equipamentos/{id}**
Remove equipamento.

## Como rodar o projeto localmente

## Rodando com Docker

### 1 - Clonar o Repositório
```bash
git clone https://github.com/PedroVieiraG/-deloitte_bootcamp_projetoFinal.git
cd MoniEquipamentosPesados
```

### 2 - Subir os Containeres
```bash
docker-compose up -d
```

Isso irá subir:
- API .NET
- SQL Server

### 3 - Verificar se está rodando
```bash
docker ps
```

### 4 - Acessar a API

Swagger:
```bash
http://localhost:8080/swagger
```

Base API:
```bash
http://localhost:8080/api/equipamentos
```

## Criando a tabela manualmente (Dbeaver ou outro cliente SQl)

### Conexão com SQL Server

Host:
```bash
localhost
```

Porta:
```bash
5433
```

Database:
```bash
equipamentos_pesados
```

Usuário:
```bash
postgres
```

Senha:
```bash
postgres
```

### Script de Criação de tabela 
```SQL
CREATE TABLE public.equipamentos_pesados (
    id               SERIAL PRIMARY KEY,
    codigo    		 VARCHAR(50) NOT NULL,
    tipo     		 VARCHAR(120) NOT NULL,
    modelo   	     VARCHAR(120) NOT NULL,
    horimetro        NUMERIC(5,2) NOT NULL, 
    status_operacional VARCHAR(50) NOT NULL, 
    data_aquisicao    TIMESTAMPTZ NOT NULL,
    localizacao_atual VARCHAR(200) NOT NULL
);
```

## Arquivo do Insomnia incluso

O repositório já contém o arquivo exportado do Insomnia com todas as requisições prontas.

### Como importar:

**1.** Abrir o Insomnia

**2.** Clicar em Create → Import

**3.** Selecionar o arquivo presente no repositório

**4.** Todas as rotas já estarão configuradas com:
- Base URL
- Body de exemplo
- Métodos corretos
- Testes prontos

## Regras de Negócio Implementadas

- Código único
- Horímetro deve ser positivo
- Data de aquisição não pode ser futura
- Tipo deve estar dentro dos valores permitidos
- Status deve estar dentro dos valores permitidos
- Validação manual no controller
- Uso de DTOs para entrada e saída
