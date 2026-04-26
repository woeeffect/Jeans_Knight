# GameInstaller

**Путь**: `Assets/Scripts/Installers/GameInstaller.cs`
**Namespace**: `Installers`
**Тип**: `MonoInstaller` (Zenject)
**На объекте**: SceneContext GameObject в сцене

## Ответственность

Регистрирует все сервисы в Zenject DI-контейнере. Единственный инсталлер проекта.

## Текущие биндинги

```csharp
Container.BindInterfacesTo<InputService>().AsSingle();
// InputService реализует IInitializable + IDisposable
// Zenject сам вызовет Initialize() на старте и Dispose() при остановке

Container.Bind<InventoryService>().AsSingle();
// InventoryService — plain class, инжектируется по типу
```

## Как добавить новый сервис

```csharp
// Сервис без MonoBehaviour:
Container.Bind<MyService>().AsSingle();

// Сервис с интерфейсом:
Container.BindInterfacesTo<MyService>().AsSingle();

// MonoBehaviour на сцене (уже существующий):
Container.Bind<MyComponent>().FromComponentInHierarchy().AsSingle();
```

## Связанные файлы

- [[Systems/Input\|InputService]] — биндится через `BindInterfacesTo`
- [[InventoryService]] — биндится как `Single`
- [[InventoryPanel]] — получает `InventoryService` через `[Inject]`
