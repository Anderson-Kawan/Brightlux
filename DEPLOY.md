# Publicação no Render

O Brightlux é uma aplicação ASP.NET Core MVC. A configuração deste projeto usa o Render para executar o .NET em um contêiner; ela não corrige o deployment na Vercel.

1. Envie os arquivos alterados para o repositório do GitHub.
2. No Render, conecte o GitHub e crie um **Blueprint** a partir do repositório Brightlux. O arquivo `render.yaml` configura o serviço e um disco persistente de 1 GB.
3. Confirme o plano e os custos exibidos pelo Render e crie o serviço. O plano Starter e o disco são pagos; o disco é necessário para preservar o banco SQLite entre deploys e reinicializações.
4. Após o deploy, abra a URL fornecida pelo Render. O banco será criado e receberá os produtos iniciais automaticamente.
5. Para criar o administrador, consulte o valor gerado de `AdminSetup__Key` nas variáveis de ambiente do serviço e acesse `/Admin/Setup`. Guarde a chave em segurança e remova a variável após criar o administrador.

O banco novo não contém os dados do arquivo SQLite local. Para preservar dados existentes, é necessário migrá-los para o disco persistente antes de usá-los em produção. O disco também guarda as chaves de autenticação; mantenha backups do diretório `/var/data`. Esta configuração usa uma única instância, pois o SQLite e o disco não são compartilhados entre instâncias.

O `.gitignore` impede que novos arquivos de compilação e bancos locais sejam adicionados, mas não remove os arquivos que já estão versionados no Git.
