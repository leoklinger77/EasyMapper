# 🧭 EasyMapper - Exemplo de Perfil de Mapeamento

Este projeto demonstra o uso do **EasyMapper**, uma alternativa simples e performática para realizar mapeamento de objetos em aplicações .NET, com foco na separação de responsabilidades e clareza.

---

## ✅ Funcionalidades

- Mapeamento fluente entre entidades e ViewModels
- Suporte à injeção de dependência
- Perfis organizados por tipo de entidade
- Permite lógica personalizada em mapeamentos

---

## 🚀 Como usar

### 1. Instale o pacote (se aplicável)

Caso esteja usando o EasyMapper como um pacote externo:

```bash
dotnet add package EasyMapper

```

### 2. Configure no Startup.cs ou Program.cs
```C#
    services.AddEasyMapper();
```

### 3. Crie seu perfil de mapeamento
```C#
public class BankMapper : MapperProfile {
    public BankMapper() {
        FromMap<Category, CategoryViewModel>(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Color, src => src.Color)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.Type, src => src.Type);

        FromMap<Category, CategoryWithSubViewModel>(dest => dest.SubCategories, src => src.SubCategories.Select(s => new SubCategoryViewModel(s)))
            .Map(dest => dest.Color, src => src.Color)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Name, src => src.Name);
}

```

### 4. Use o mapeamento.
```C#
public class TransactionService
{
    private readonly IMapper _mapper;

    public TransactionService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TransactionsViewModel Convert(Transactions model)
    {
        return _mapper.Map<Transactions, TransactionsViewModel>(model);
    }
}

```

### 🧪 Testes
Você pode testar o mapeamento diretamente com injeção de dependência, ou criar testes unitários mockando os objetos de entrada e comparando os valores esperados.

### 📚 Licença
Este projeto está licenciado sob a MIT License.

### 🤝 Contribuindo
Pull requests são bem-vindos! Para mudanças importantes, abra uma issue primeiro para discutir o que você gostaria de mudar.
