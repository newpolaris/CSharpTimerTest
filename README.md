Timer �� �ػ� Ȯ�� �� callback �� thread Ȯ���� ���� �׽�Ʈ ������Ʈ

MFC�� SetTimer, KillTimer�� �ػ󵵰� ���� ������, main thread ���� ȣ��ȴ�. 
��� �� �� ������, �Ƹ� ���� ���� ������ main thread �� ���̴�

C#�� Timer

https://web.archive.org/web/20150329101415/https://msdn.microsoft.com/en-us/magazine/cc164015.aspx

Timer event runs on what thread? ��� �Ǿ��ִ� ���ǿ� �����ϰ� ����Ǿ� �ִ�.

�ش� ǥ���� System.Timers �� UI or Work thread �� �Ǿ��ִµ�,

UI�� ���� synchronizeObject ����? Sync �ɼ��� ���� ���̴�.

�ػ� ���� �ڼ��� �׽�Ʈ�� �Ʒ���,

https://www.codeproject.com/articles/17474/timer-surprises-and-how-to-avoid-them

multimedia �� import �ؼ� ��� ms ������

��Ƽ�̵�� ���� ���ļ�, System.Thread.Timer, System.Timers.Timer ��� ���� thread ����̰�,

Thread.Timer�� work thread�� �ϳ��� �ƴ϶� ���������� ȣ�� �� �� ����. 
break �ɰ� �׽�Ʈ�ϴ°� work thread�� 2�� ~ 3�� ���� �ô���

DispatcherTimer�� UI �� queue �����̶� �� ��Ư�ϰ� ����, �̰͵� 16 ms �� �Ѱ�
������ �����ϰ�, UI thread ���� ȣ������

UI���� ȣ���ϴ� ���� com �̶��, com �� dll �� ���� main thread ���� ȣ���

�̷���, Thread.Timer �� ���, ���� work thread���� engine �Լ� ȣ��� serialize �Ǳ� ������
callback ���� ó���� ��� �̷�� ���� �ʴ� ��찡 ������.

timer ���� �� 1 ���� ��ɿ� �������� ��, Ÿ�̸ӷ� �̹� ������ 2��° ����� ���� ȣ�� ������̶�,
ȣ��ǰ� �ȴ�.

��Ƽ������ �ڵ��� �߸��Ȱű� �ѵ�, �ٸ� platform ���� �ٸ� ������ ����
