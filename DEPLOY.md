# Publicação na Vercel

O projeto usa o runtime de contêiner da Vercel (em beta), configurado em `vercel.json`, e PostgreSQL para guardar produtos, contas e chaves de autenticação. O SQLite continua disponível para desenvolvimento local e para a configuração existente do Render.

1. Crie um banco PostgreSQL externo e obtenha os dados de conexão. Use uma conexão direta ou um pool em modo de sessão: a inicialização usa um bloqueio PostgreSQL por sessão para evitar migrações e produtos duplicados.
2. Importe o repositório na Vercel e selecione o preset **Container**. O projeto usa o `Dockerfile` da raiz. Se o projeto já existir, confira nas configurações se não há comandos de build ou diretório de saída personalizados substituindo a configuração do repositório.
3. Configure as variáveis de ambiente da Vercel para o ambiente desejado:
   - `ConnectionStrings__Postgres`: string Npgsql, por exemplo `Host=SEU_HOST;Port=5432;Database=SEU_BANCO;Username=SEU_USUARIO;Password=SUA_SENHA;SSL Mode=Require`. Use as configurações de TLS indicadas pelo provedor. Não coloque a senha no repositório.
   - `AdminSetup__Key`: uma chave longa e aleatória para proteger a criação inicial do administrador.
4. Faça o deploy. Na primeira inicialização, a aplicação cria as tabelas e os produtos iniciais. Acesse `/Admin/Setup`, informe a chave de configuração e crie o administrador. Depois, remova `AdminSetup__Key` e faça um novo deploy.

O banco PostgreSQL começa vazio: produtos e usuários do arquivo SQLite local não são importados automaticamente. Faça uma migração de dados e backup antes de substituir uma instalação existente. As chaves de autenticação ficam no PostgreSQL sem criptografia adicional pela aplicação; proteja o acesso e os backups do banco. O runtime de contêiner usa cobrança e limites da Vercel; confira o plano antes de publicar.

Para executar com PostgreSQL localmente, defina `ConnectionStrings__Postgres` e inicie com `dotnet run`. Sem essa variável, a aplicação usa a conexão SQLite de `appsettings.json`. Para criar migrações PostgreSQL, use `dotnet ef migrations add Nome --context PostgresDbContext --output-dir Migrations/Postgres`; as migrações SQLite continuam vinculadas a `ApplicationDbContext`.

## Render

O `render.yaml` continua disponível para publicação com Docker e disco persistente. O plano e o disco podem ser pagos. Consulte as variáveis configuradas no arquivo e crie um Blueprint no Render. Faça backups do diretório `/var/data`, que contém o SQLite e as chaves de autenticação.
