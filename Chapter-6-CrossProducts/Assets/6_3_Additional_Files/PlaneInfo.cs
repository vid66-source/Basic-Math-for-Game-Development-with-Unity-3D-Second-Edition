using UnityEngine;

internal struct PlaneInfo{
    // Поля структури повинні бути ініціалізовані
    // і позначаються як readonly, оскільки це поля конструктора.
    public readonly Vector3 P0, P1, P2;

    private readonly Vector3 _normal;


    // 1. V1 - обчислювана властивість (тільки для читання)
    public Vector3 V1{
        get{ return P1 - P0; }
    }

    // 2. V2 - обчислювана властивість
    public Vector3 V2{
        get{ return P2 - P0; }
    }

    // 3. Normal - обчислювана нормаль площини
    public Vector3 Normal => _normal; // Просто повертаємо

    // 4. Властивість D (Константа площини P dot vn = D)
    // Аксесор get обчислює D, використовуючи внутрішні поля.
    public float D{
        get{ return Vector3.Dot(Normal, P0); }
    }

    // 5. Конструктор
    // В структурі конструктор повинен ініціалізувати ВСІ readonly поля.
    public PlaneInfo(Vector3 p0, Vector3 p1, Vector3 p2){
        P0 = p0;
        P1 = p1;
        P2 = p2;
        _normal = Vector3.Cross(p1 - p0, p2 - p0).normalized; // Обчислюємо 1 раз
    }
}