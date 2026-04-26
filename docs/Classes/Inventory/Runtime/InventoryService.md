# InventoryService

**Путь**: `Assets/Scripts/Inventory/Runtime/InventoryService.cs`
**Namespace**: `Inventory.Runtime`
**Тип**: plain class (не MonoBehaviour)
**DI**: биндится как `Single` в [[GameInstaller]]

## Ответственность

Единственная точка входа для операций с инвентарём игрока. Хранит `PlayerInventory` и предоставляет его UI и любому коду через DI.

## Свойства

| Свойство | Тип | Описание |
|---|---|---|
| `PlayerInventory` | `Inventory` | Инвентарь игрока (N слотов) |

## Как получить в коде

Через Zenject `[Inject]`:
```csharp
[Inject]
public void Construct(InventoryService inventoryService)
{
    _inventoryService = inventoryService;
}
```

## Capacity (размер инвентаря)

Задаётся в конструкторе: `new InventoryService(capacity: 20)`.
Если нужно изменить — поправь значение в [[GameInstaller]] при биндинге или добавь сериализованное поле.

## Класс Inventory

`PlayerInventory` — это экземпляр `Inventory.cs`:

| Метод | Описание |
|---|---|
| `TryAdd(ItemDataSO, count)` | Добавить предмет (false если нет места) |
| `Remove(ItemInstance)` | Убрать конкретный экземпляр |
| `GetAt(int index)` | Получить по индексу |
| `Items` | IReadOnlyList — все предметы |
| `Count` / `Capacity` | Текущий счётчик / максимум |

## Связанные файлы

- [[InventoryPanel]] — UI, инжектирует этот сервис
- [[Classes/Combat/Controllers/EquipmentController\|EquipmentController]] — принимает предметы из инвентаря на экипировку
- [[GameInstaller]] — место биндинга
