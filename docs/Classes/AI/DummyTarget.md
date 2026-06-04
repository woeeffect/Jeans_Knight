# DummyTarget

**Путь**: `Assets/Scripts/AI/DummyTarget.cs`
**Namespace**: `AI`
**На объекте**: тестовый манекен на сцене

## Ответственность

Тестовая цель для проверки боевой системы. Реагирует на урон (вспышка цвета), на смерть (уничтожение), на оглушение (жёлтый цвет). Предоставляет Debug-атаку по игроку для тестирования блока/парри.

## Реализуемые интерфейсы

| Интерфейс | Зачем |
|---|---|
| `IDamageReceivedHandler` | Получить событие при уроне (через [[Systems/EventBus\|EventBus]]) |
| `IDeathHandler` | Получить событие смерти |
| `IStunnable` | Быть оглушённым при парировании атаки игроком |

> `DummyTarget` получает событие `OnDamageReceived` **на любой объект**, не только себя — поэтому первая строка каждого обработчика: `if (victim != gameObject) return;`

## Параметры (Inspector)

| Группа | Поле | Описание |
|---|---|---|
| Visuals | `targetRenderer` | Renderer для цветовых эффектов |
| Visuals | `hitFlashColor` | Цвет при ударе (красный) |
| Visuals | `stunColor` | Цвет при оглушении (жёлтый) |
| Visuals | `hitFlashDuration` | Длительность вспышки (сек) |
| Debug Attack | `playerHealth` | HealthComponent игрока (авто-поиск если пусто) |
| Debug Attack | `debugAttackDamage` | Урон отладочной атаки |
| Debug Attack | `debugAttackType` | Тип урона (Physical / Magic) |

## Debug-атака (Context Menu)

В Inspector кнопка **"Debug: Attack Player"** — симулирует удар по `playerHealth` с параметрами выше.
Используется для тестирования блока и парри без вражеского AI.

```csharp
[ContextMenu("Debug: Attack Player")]
public void DebugAttackPlayer()
```

## Что изменить здесь при разработке врагов

Когда появятся настоящие враги, этот класс станет **шаблоном** для их реакций. Можно расширить:
- `OnDamageReceived` → проиграть анимацию hit.
- `Stun()` → перейти в стейт оглушения (если у врага будет стейт-машина).
- `OnDeath` → дроп лута, начисление очков.

## Связанные файлы

- [[HealthComponent]] — компонент HP на том же объекте
- [[BlockController]] — вызывает `Stun()` при успешном парировании
- [[Systems/Combat]] — полный поток урона
