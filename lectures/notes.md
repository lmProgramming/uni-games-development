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

## blender

maya - drogie ale dobre, dobre do filmów animowanych np. Pixara

autodesk 3ds max - drogie, tylko widnows, prostsze niż maya, dobre do gier

blender - darmowe, open source, modelowanie, sculptowanie, animacje, ma silnik gier??

### principles of animation

1. timing and rhythm
2. spacing
3. squash and stretch - piłka rozszerza się uderzając o podłogę
4. arcs
5. anticipation - akcja i reakcja np. bejsbolista ręka do tylu zanim uderzy
6. drag and follow through - realistyczna fizyka
7. asymetria
8. overlapping action
9. secondary action
10. exaggeration - przesada xd wyolbrzymienie
11. consistency
12. staging and clarity
13. readability and focus

## gry 3D

whiteboxing - przygotowanie sceny najpierw z białych prostopadłościanów

### lighting

directed, point - z punktu wokół w sferze, spot - stożek/reflektor

cookie - filtr co zmienia sposób emisji "kształt" światła - np. brud na drodze światła

### pytania kontrolne 5

- Wymień rodzaje źródeł światła czasu rzeczywistego dostępne w Unity
    Directed, Point, Spot
- Zdefiniuj pojęcia: tekstura, materiał, zacieniacz; jakie są między nimi relacje?
    Tekstura - grafika nałożona na model
    Shader - kod co każdy piksel kalkuluje
    Materiał - połączenie shadera z jego opcjami jak tekstura i inne
- Czym jest ciasteczko (cookie)?
    zmienia kolor i wzór emitowanego światła
    np. symulacja cienia rzucanego przez kratę w oknie, liście drzewa, brud na reflektorze
- Czym jest Skybox
    taki sześcion co otacza scenę i niebo udaje
- Czym jest system cząsteczek? Podaj sytuację, gdy może mieć zastosowanie
    kontroluje i tworzy dużo małych cząsteczek
    np. do krwi
- Czym jest Raycasting? Do czego może mieć zastosowanie?
    strzelanie
- Wymień główne elementy wbudowanego systemu nawigacji AI
    NavMeshSurface - komponent do dodania do sceny, NavMesh - już wygenerowana powierzchnia, NavMeshAgent, NavMeshObstacle - blokada, NavMeshLink - skok

## gry 3D 2

frustum culling - poza bryłą widzenia

occlussion culling - za innymi obiektami (occluder - zasłaniający, ocludee - zasłaniany)

backface culling - odrzucanie back faces

blend trees - drzewa mieszające - maszyna stanów mieszająca np. bieganie i chód w zależności od prędkości

ScriptableObject - kontener danych, może być zapiasny jako x.asset, CreateAssetMenu(filename, menuname)

ragdoll: wyłącz animator, dla każdej kończyny wyłącz isKinematic, włącz collider, grawitację - automatycznie robi to Create Ragdoll

### pytania kontrolne 6

- Jakie typowe zadania można wykonać w trakcie modelowania terenu?
    tworzenie sąsiadujących kafelków terenu
    rzeźbienie i malowanie terenu
    malowanie teksturami
    drzewa, roślnność
    wind zone dla roślniek
- Wymień przykładowe szumy, które mogą być wykorzystane przy generacji terenu i biomów
    perlin i simplex
- Wymień najważniejsze parametry kamery
    clearFlag, orthographic, fieldView, depth
- Na czym polega Occlusion Culling?
    nie renderowanie obiektów które są za innymi
- Do czego służą drzewa mieszające? Jakie typy drzew mieszających można zdefiniować w Unity
    do mieszania ze sobą animacji; (chodzi o liczbę parametrów) 1D, 2D, Direct (mapa wag)
- Do czego w Unity służy kwaternion? Jakie operacje najczęściej na nim się wykonuje?
    rotacja; Euler(), Slerp(), Identity, mnożenie q1 x q2, q1 x v1 (obracanie)
- Czym jest Ragdoll?
    technika animacji proceduralnej
- Do czego służy obiekt skryptowy? Jakim podlega ograniczeniom?
    kontener na dane; nie jest to komponent

## sztuczna inteligencja

### pytania kontrolne 7

- Jak można zrealizować inteligencję w grze (w deterministyczny sposób)?
    drzewa decyzyjne, zachowań, maszyny stanów, GOAP, fuzzy logic
- Opisz kluczowe elementy drzewa decyzyjnego
    Składowe drzewa:
        Węzły decyzyjne: Wewnętrzne węzły drzewa, w których podejmowana jest decyzja na podstawie stanu gry lub agenta.
        Decyzje (liście): Końcowe węzły drzewa, które reprezentują konkretną akcję do wykonania przez agenta.
        Etykietowane przejścia: Krawędzie łączące węzły, gdzie każda etykieta odpowiada jednemu z możliwych wyników decyzji w węźle nadrzędnym.
    Sposób działania:
        Agent, aby podjąć decyzję, za każdym razem przechodzi całe drzewo od korzenia w dół, podążając za przejściami zgodnymi z aktualnym stanem, aż dotrze do liścia (akcji).
    Implementacja:
        Może być zrealizowana za pomocą sekwencji instrukcji warunkowych if lub dedykowanych klas reprezentujących węzły i przejścia.
- Opisz kluczowe elementy drzewa zachowań
    Typy węzłów:
        Korzeń (Root): Punkt startowy drzewa, który wysyła sygnały (tzw. "ticki") do swoich dzieci w regularnych odstępach czasu.
        Węzły sterujące (Control Nodes): Węzły posiadające co najmniej jedno dziecko, które sterują przepływem wykonania. Przykłady:
            Sekwencja (Sequence, ->): Wykonuje dzieci po kolei i zwraca sukces tylko, gdy wszystkie dzieci zwrócą sukces.
            Selektor (Selector, ?): Wykonuje dzieci po kolei, aż jedno z nich zwróci sukces lub running. Zwraca failure, jeśli wszystkie dzieci zawiodą.
            Dekorator (Decorator): Modyfikuje działanie lub wynik swojego jedynego dziecka (np. Inwerter).
            Zrównoleglacz (Parallel): Wykonuje swoje dzieci jednocześnie.
        Węzły wykonawcze (Execution Nodes): Liście drzewa, które reprezentują konkretne akcje (np. Atakuj) lub warunki (np. CzyGraczBlisko?).
    Statusy zwrotne: Węzły wykonawcze po każdym "ticku" zwracają jeden z trzech statusów, które determinują dalsze działanie drzewa:
        Success: Operacja zakończyła się powodzeniem.
        Failure: Operacja zakończyła się niepowodzeniem.
        Running: Operacja jest w toku i wymaga więcej czasu na ukończenie.
- Opisz kluczowe elementy maszyny stanów
    stan, przejścia między stanami, ewentualnie akcje OnStart, OnExit
- Opisz kluczowe elementy GOAP
    Agent, Goal, Actions [Warunki wstępne, końcowe, koszt], Planer
- W jaki sposób można w Unity zapisywać/odczytywać dane?
    Klasycznie JsonUtility.ToJson i File.WriteAllText

## do nauczenia

system UI Unity

2 abcd?
