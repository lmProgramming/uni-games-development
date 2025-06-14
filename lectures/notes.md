# notes

## podstawy Unity

### UI typy

UI Toolkit - jak CSS HTML, najnowszy

Unity UI - klasyczne Buttony itd., prosty

IMGUI - wymaga pisania kodu

HUD to GUI nałożone na widok gry

### event system

można dodać komponent Event Trigger, który ma listę w sobie eventów które handlure invoke'ując metody w innych komponentach

można własny event zrobić jak:

```cs
public interface TestInterface: IEventSystemHandler{
    void Message(string txt);
}
public class TestController: MonoBehaviour, TestInterface {
    public void Message(string txt) { Debug.Log(txt); }
}

ExecuteEvents.Execute<TestInterface>(target, "test", (x, y) => x.Message(y));
```

## new input system

gracz -> urządzenie -> interakcja -> akcja -> metoda akcji

## pytania kontrolne 2a

- jakimi terminami posługuje się tzw. nowy system wejścia?
    InputActionMap, InputAction, InputActionAsset, InputBinding
- jak zrealizować korutynę?
    IEnumerator MyCoroutine(int x) { yield return new WaitForSeconds(x); Debug.Log("done!") }
    StartCoroutine(MyCoroutine(3));
- jak uruchomić funkcję asynchroniczną?
    async void x() { Task.Yield(); Task.Delay(3); }
    async jest wywoływany jak normalna funkcja, np. x() lub await x() jak chcemy czekać
- jakie są podstawowe różnice między coroutine a funkcją async?
    korutyna zawsze się kończy, też gdy obiekt jest zniszczony
    async może zwrócić wartość
    korutyny są tylko w Unity
    słaba obsługa wyjątków korutyny, brak stosu wywołań
    async natywnie wspiera więcej opcji czekania na x
    async jest wywoływany jak normalna funkcja, np. x() lub await x() jak chcemy czekać
    korutyna y = StartCoroutine(x()); StopCoroutine(y); StopAllCoroutines()

## Physics2D

komponent ConstantForce dla Rigidbody

jak kinematic, to inne się odbijają od niego on sam nie odbija się

HingeJoint2D - zawias

SpringJoint2D - sprężyna

### Effectors

PlatformEffector2D - przechodzenie przez platformę tylko w jednym kierunku

PointEffector2D - czarna dziura, wybuch

SurfaceEffector2D - taśmociąg

AreaEffector2D - wchodzenie po pionowej ścianie

Collider musi być wtedy połaczony z Effectorem (collider.usedByEffector)

### pytania kontrolne 2

- Jakie komponenty z pakietu Physics zostały omówione na wykładzie?
    RigidBody, Collider, Effector, PhysicsMaterial, Joint
- Jakie cechy ma kinematyczna bryła sztywna?
    Nie podlega siłom
    Inne rzeczy się od niej odbijają
    Nie reaguja na kolizje, ale je wykrywa
    Jest sterowana przez skrypt/animację
- Czym się różni zderzacz, który jest wyzwalaczem (IsTrigger) od takiego, który nie jest?
    IsTrigger nie zmienia toru ruchu rzeczy, wywołuje tylko funkcje jak OnTriggerEnter2D
- Czym jest efektor? Podaj 2 przykłady
    Wchodzenie po ścianach, przechodzenie przez platformę tylko z 1 strony
- Jaka jest różnica między funkcjami FixedUpdate oraz Update?
    FixedUpdate wykonywane zawsze tyle samo razy na sekundę, Update do Inputu jest
- W jaki sposób można przenosić dane między scenami?
    DontDestroyOnLoad, PlayerPrefs, statyczne pola

## Animations

Mecanim - nazwa systemu w Unity

podstawowe: AnimationClip, AnimationController, Animator

Animator to most między prostym klipem a AnimationController z maszyną stanów

AnimationEvent to event jak każdy inny wywołany w danym momencie na jakimś obiekcie na jakiejś metodzie

można dla stanu w AnimatioNCotrnoller "Add Behaviour" i to tworzy np. WaitingBehaviour : StateMachineBehaviour

### IMGUI

np.

```cs
GUI.Label(new Rect(25, 25, 25, 25), "Label");

if (GUI.Button(new Rect..., 'Button')) {...}

if (GUI.RepeatButton)

var text = ""
text = GUI.TextField(rect..., text)

var toggleBool = GUI.Toggle(rect..., true, "Toggle");

var hsliderValue = 0.0f;
hsliderValue = GUI.HorizontalSlider(rect..., hSliderValue, 0, 100);

var hScrollbarValue = 0.0f;
hScrollbarValue = GUI.HorizontalScrollbar(rect..., hSliderValue, howMuchCanWeSee=1f, 0, 100) 
```

### pytania kontrolne 3

- Jakie są podstawowe elementy systemu Mecanim? Jakie istnieją pomiędzy nimi relacje?
    AnimationClip, AnimationController, Animator
    Animator to most między prostym klipem a AnimationController z maszyną stanów
- Czym może być warunkowane przejście na maszynie stanów w kontrolerze animacji?
    ExitTime, trigger, zmiana wartości int float bool
- Wymień podstawowe elementy pozwalające na odtwarzanie dźwięków w Unity?
    SoundClip w AudioSource
    AudioListener - uszy gracza
- Czym jest IMGUI? Jaką metodę trzeba przykryć, aby z tego trybu skorzystać?
    to GUI z kodu C#;
    void OnGUI() {}
- Czym jest PCG? Co może być generowane?
    Procedural Context Generation; Poziomy, unikalne przedmioty, muzykę, fabułę

## do nauczenia

system UI Unity

2 abcd?
