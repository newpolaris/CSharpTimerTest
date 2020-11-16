Timer 별 해상도 확인 및 callback 의 thread 확인을 위한 테스트 프로젝트

MFC의 SetTimer, KillTimer는 해상도가 낮긴 하지만, main thread 에서 호출된다. 
몇번 본 것 같은데, 아마 현재 엔진 구조상 main thread 일 것이다

C#의 Timer

https://web.archive.org/web/20150329101415/https://msdn.microsoft.com/en-us/magazine/cc164015.aspx

Timer event runs on what thread? 라고 되어있는 섹션에 간략하게 기술되어 있다.

해당 표에선 System.Timers 가 UI or Work thread 로 되어있는데,

UI의 경우는 synchronizeObject 였나? Sync 옵션을 켰을 때이다.

해상도 관련 자세한 테스트는 아래에,

https://www.codeproject.com/articles/17474/timer-surprises-and-how-to-avoid-them

multimedia 쪽 import 해서 써야 ms 단위임

멀티미디어 까지 합쳐서, System.Thread.Timer, System.Timers.Timer 모두 별도 thread 기반이고,

Thread.Timer는 work thread가 하나가 아니라 여러개에서 호출 될 수 있음. 
break 걸고 테스트하는게 work thread가 2개 ~ 3개 까지 늘더라

DispatcherTimer는 UI 에 queue 형식이라 좀 독특하게 동작, 이것도 16 ms 가 한계
사용법이 간단하고, UI thread 에서 호출해줌

UI에서 호출하는 쪽이 com 이라면, com 쪽 dll 을 통해 main thread 에서 호출됨

이러면, Thread.Timer 의 경우, 여러 work thread에서 engine 함수 호출시 serialize 되기 때문에
callback 관련 처리가 즉시 이루어 지지 않는 경우가 존재함.

timer 끄는 걸 1 번쨰 명령에 수행했을 때, 타이머로 이미 시작한 2번째 명령은 엔진 호출 대기중이라,
호출되게 된다.

멀티쓰레드 코딩이 잘못된거긴 한데, 다른 platform 과는 다른 동작을 보임
