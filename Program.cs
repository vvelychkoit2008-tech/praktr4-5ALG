namespace Demos.RoundRobin;

public static class RoundRobinListDemo
{
    public static void Run()
    {
        var list = new RoundRobinList();
        list.Add(1);
        list.Add(0);
        list.Add(2);
        //прокручуєм циклічний список декілька разів
        for (var i = 0; i < 10; i++)
        {
            Console.Write(list.CurrentValue);
            list.MoveBack();
            if (i < 9)
                Console.Write(",");
        }
        Console.WriteLine();
        //свій код писати можна лише в цьому методі
        //встановити  список на мінімальний елемент
        //ваш алгоритм має бути тут
        //СПОСІБ 1: обхід циклу один раз для пошуку мінімального
        int min = list.CurrentValue.Value;
        for (int i = 0; i < list.Count; i++)
        {
            if (list.CurrentValue < min)
                min = list.CurrentValue.Value;
            list.MoveNext();
        }
        // тепер повертаємось до мінімального елемента
        for (int i = 0; i < list.Count; i++)
        {
            if (list.CurrentValue == min)
                break;
            list.MoveNext();
        }
        var minValue = list.CurrentValue; //очікуваний результат 0
        Console.WriteLine("СПОСІБ 1 — мінімальний елемент: " + minValue);
        //СПОСІБ 2: обхід у зворотному напрямку для створення масиву
        int[] array = new int[list.Count];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            array[i] = list.CurrentValue.Value;
            list.MoveBack(); // йдемо у зворотному напрямку
        }
        Console.WriteLine("СПОСІБ 2 — масив у зворотньому порядку: " + string.Join(',', array)); //очікуваний результат 0,1,2
        // ЗАВДАННЯ 4:
        /* В методі RoundRobinListDemo.Run() в зазначеному місці дописати свої алгоритми, які виконують наступну логіку:
         1. Встановити активний елемент списку в елемент з найменшим значенням.
         2. Перевести значення елементів списку в масив, обійшовши всі елементи списку в зворотньому порядку. */
        //1. Знайти мінімальний елемент вручну
        int minValueTask4 = list.CurrentValue ?? 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list.CurrentValue < minValueTask4)
                minValueTask4 = list.CurrentValue.Value;
            list.MoveNext();
        }
        //2. Перейти на елемент із мінімальним значенням
        for (int i = 0; i < list.Count; i++)
        {
            if (list.CurrentValue == minValueTask4)
                break;
            list.MoveNext();
        }
        Console.WriteLine("ЗАВДАННЯ 4 — мінімальний елемент списку: " + list.CurrentValue);

        //3. Обійти всі елементи у зворотньому порядку й записати у масив
        int[] backwardArray = new int[list.Count];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            backwardArray[i] = list.CurrentValue.Value;
            list.MoveBack();
        }
        Console.WriteLine("ЗАВДАННЯ 4 — список у зворотньому порядку: " + string.Join(",", backwardArray));
    }
}

public class DoubleLinkedNode
{
    public int Value { get; set; }
    public DoubleLinkedNode? Next { get; private set; }
    public DoubleLinkedNode? Prev { get; private set; }

    protected DoubleLinkedNode(int value)
    {
        this.Value = value;
    }
    protected void SetNext(DoubleLinkedNode newNode)
    {
        //запамятаємо залишкові елементи
        var tmpNext = this.Next;

        //прив'яжемо новий елемент з основним списком
        this.Next = newNode;
        newNode.Prev = this;

        //якщо залишок не пустий
        if (tmpNext != null)
        {
            //прив'язати залишок до списку після вставленого елемента
            newNode.Next = tmpNext;
            tmpNext.Prev = newNode;
        }
    }
}

public class RoundRobinList
{
    private class InternalNode : DoubleLinkedNode
    {
        public InternalNode(int value) : base(value)
        {
        }
        public void SetNext(InternalNode node)
        {
            base.SetNext(node);
        }
    }
    private int _count;
    private InternalNode? _pointerNode;
    public int Count
    {
        get
        {
            return _count;
        }
    }

    //додає новий елемент до циклічного списку
    public DoubleLinkedNode Add(int value)
    {
        var newNode = new InternalNode(value);
        if (_pointerNode == null)
        {
            _pointerNode = newNode;
            newNode.SetNext(newNode);
        }
        else
        {
            _pointerNode.SetNext(newNode);
            _pointerNode = newNode;
        }
        _count++;

        return newNode;
    }

    //виводить значення активного елемента списку
    public int? CurrentValue
    {
        get
        {
            if (_pointerNode == null) return null;
            return _pointerNode.Value;
        }
    }

    //активує наступний елемент
    public DoubleLinkedNode? MoveNext()
    {
        if (_pointerNode != null)
        {
            _pointerNode = (InternalNode)_pointerNode.Next;
            //this.SetPointerNode(_pointerNode!.Next);
        }
        return _pointerNode;
    }

    //активує попередній елемент
    public DoubleLinkedNode? MoveBack()
    {
        if (_pointerNode != null)
        {
            _pointerNode = (InternalNode)_pointerNode.Prev;
            //this.SetPointerNode(_pointerNode!.Prev);
        }

        return _pointerNode;
    }

    //встановлює активний елемент
    protected bool SetPointerNode(DoubleLinkedNode node)
    {
        if (_pointerNode == null) return false;
        if (node == _pointerNode) return true;
        var currentNode = (InternalNode)_pointerNode.Next;
        while (currentNode != _pointerNode)
        {
            if (currentNode == node)
            {
                _pointerNode = currentNode;
                return true;
            }
            currentNode = (InternalNode)currentNode.Next;
        }
        return false;
    }

    //повертає елемент з найменшим значенням
    protected DoubleLinkedNode? FindMin()
    {
        if (_pointerNode == null) return null;
        //запамятаємо елемент з якого починаємо рух
        var startNode = _pointerNode;
        //нехай найменший елемент це наш перший елемент
        var minNode = _pointerNode;
        //встановимо поточну ноду на наступний елемент
        var currentNode = startNode.Next;
        //поки не зробили повний оберт по всім елементам списоку
        while (currentNode != startNode)
        {
            //для кожного елементу перевіряємо чи поточний елемент менший за мінімальний
            if (currentNode!.Value < minNode.Value)
            {
                //якщо так то мінімальний елемент це наш поточний
                minNode = (InternalNode)currentNode;
            }
            //переходимо на наступний елемент
            currentNode = currentNode.Next;
        }
        return minNode;
    }

    public int[] ToArrayForvard()
    {
        // алгоритм що перетворить циклічний список у масив,
        // обійшовши список в прямому порядку
        throw new NotImplementedException();
    }
    public int[] ToArrayBackward()
    {
        // алгоритм що перетворить циклічний список у масив,
        // обійшовши список в зворотньому порядку
        throw new NotImplementedException();

    }
}
class Program
{
    public static void Main()
    {
        RoundRobinListDemo.Run();
    }
}