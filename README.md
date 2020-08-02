# README

# Prototype-refactory

그 기업반 프로젝트 그거를 리팩토링한 프로젝트 입니다

## 코드 작성 권장사핳

---

- 멤버변수는 코드의 두곳 이상에서 변수를 사용하지 않는 이상, 지역 변수로 대체합니다.
- 멤버변수는 대문자로 시작합니다. **private**이나 **protected**변수의 경우 접두사 **m**을 사용합니다.

```csharp
public  int  FloorIndex;
private int mFloorIndex;
```

- 간단한 **get** 메서드 변수라면, 람다식을 사용합니다.

```csharp
public  int  FloorIndex => mFloorIndex;
private int mFloorIndex;
```

- 코루틴을 비롯한 **IEnumerator,** **IEnumerable**은 접두사 **E**를 사용합니다.

```csharp
private IEnumerator EMove() { ... }
```

- **IEnumerator,** **IEnumerable** 멤버변수 또한 접두사 **m**을 사용합니다.

```csharp
private IEnumerator mEMove;
```

- 변수나 객체의 초기화는 선언과 동시에 이루어지게 되면 코드가 지저분해 보이므로, 
선언과 초기화를 분리합니다.

```csharp
// X
private List<GameObject> mObjectList = new List<GameObject>();
```

```csharp
// O
private List<GameObject> mObjectList;

private void Start()
{
	mObjectList = new List<GameObject>();
}
```

- 인스펙터창에서 값을 수정할 수 있도록 설계한 변수들은 **private**이나 **protected**를 사용한 뒤,
씨리얼라이즈합니다.

```csharp
[SerializeField] private GameObject SwitchingObject;
```

---

## 커밋 권장사항

---

![README.png](README/(696).png)

원문 : [https://item4.blog/2016-10-31/How-to-Write-a-Git-Commit-Message/](https://item4.blog/2016-10-31/How-to-Write-a-Git-Commit-Message/)