C# .NET 4.5.2 решение состоит из двух проектов 

проект CN2 библиотека классов, в которой определены алгоритмы машинного обучения и реализованы предметные модели, необходимые для работы алгоритмов
ключевые компоненты:
  класс CN2.Core.Algorithms.CN2
    реализованы алгоритмы машинного обучения
  класс CN2.Core.DataStructures.AttributeType
    модель типа атрибута
  класс CN2.Core.DataStructures.LearnableExample
    модель обучающего примера
  класс CN2.Core.DataStructures.ArguedLearnableExample
    модель агрументированного обучающего примера
  класс CN2.Core.DataStructures.ProductionRule
    модель продукционного правила
  класс CN2.Core.DataStructures.ExaminableExample
    модель примера, поддерживающая проведение экзамена
  интерфейс CN2.Core.DataStructures.IExpressionMember
    обобщение члена логического выражения
  класс CN2.Core.DataStructures.Expression
    модель логического выражения
  класс CN2.Core.DataStructures.AttributeValue
    модель конкретного значения атрибута
  пространство имён CN2.UC
    модели представления и представления контекстных сущностей 

проект WpfApp Windows приложение, построенное с использованием WPF для демонстрации функций библиотеки
