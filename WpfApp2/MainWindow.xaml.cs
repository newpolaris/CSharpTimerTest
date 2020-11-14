using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer m_dispatcher;
        private System.Timers.Timer m_timer;
        private Thread m_thread;
        private TextBlock m_fpsCounter;
        private DateTime m_lastUpdate;
        private int fpsCounter;
        private double frameTime;
        private uint m_fastTimer;

        private TimerEventHandler timerHandler;

        private Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // https://www.codeproject.com/articles/17474/timer-surprises-and-how-to-avoid-them
            // Form.Timer, Thread.Timer, Timers.Timer gives 16 ms minimum interval.
            // multimedia timer gives 1 ms interval
                
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/2ccd207d-2fbb-4070-8bdc-4ab26e69ded7/dispatchertimer-doesnt-seem-to-work-with-milliseconds?forum=wpf
            // limited with UI thread ~ 16 ms (deps on UI setting maybe)
            m_dispatcher = new DispatcherTimer(DispatcherPriority.Send);
            m_dispatcher.Interval = TimeSpan.FromMilliseconds(1);
            m_dispatcher.Tick += new EventHandler(OnTick);
            // m_dispatcher.Start();

            // can run on multiple worker thread and limited resolution (> 16 ms)
            // request 20 ms > 33 fps, request 10 ms > 64 fps, request 5 ms > 64 fps
            // (http://blog.daum.net/starkcb/117  - use SynchronizingObject to restrict UI thread)
            m_timer = new System.Timers.Timer(5);
            m_timer.Elapsed += OnRun;
            // m_timer.Start();

            // no limit (if Thread.Sleep accurate)
            m_thread = new Thread(new ThreadStart(OnThreadRun));
            // m_thread.Start();

            TimeCaps timeCaps = new TimeCaps(0, 0);
            // timerSetEvent has it's own thread.
            uint result = timeGetDevCaps(out timeCaps, Marshal.SizeOf(timeCaps));
            if (result == 0 /*TIMERR_NOERROR*/) {}


            int msDelay = 1;
            int msResolution = 1;
            int myData = 0; // dummy data

            // https://www.sysnet.pe.kr/2/0/710
            timerHandler = new TimerEventHandler(tickHandler);
            m_fastTimer = timeSetEvent(msDelay, msResolution, timerHandler, ref myData, 1);

            m_fpsCounter = new TextBlock();
            m_fpsCounter.Margin = new Thickness(3);
            m_fpsCounter.VerticalAlignment = VerticalAlignment.Bottom;

            Main.Children.Add(m_fpsCounter);
        }

        public void OnTick(Object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - m_lastUpdate;

            if (elapsed.TotalMilliseconds >= 1000)
            {
                m_fpsCounter.Text = "FPS= " + fpsCounter.ToString();
                fpsCounter = 0;
                m_lastUpdate = DateTime.Now;
            }
            fpsCounter++;
        }

        public void OnThreadRun()
        {
            while (run())
            {
                // https://www.codeproject.com/articles/17474/timer-surprises-and-how-to-avoid-them
                // section: Thread.Sleep
                // sleep and restart requires > 0.9 ms (?)
                Thread.Sleep(3);
            }
        }

        public void OnRun(Object sender, EventArgs e)
        {
            run();
        }

        public bool run()
        {
            stopwatch.Restart();
            TimeSpan elapsed = DateTime.Now - m_lastUpdate;

            if (elapsed.TotalMilliseconds >= 1000)
            {
                string fps = "FPS= " + fpsCounter.ToString() + " / frametime: " + frameTime + " ms";
                Action<string> action = delegate (string text) {
                        m_fpsCounter.Text = text;
                };
                fpsCounter = 0;
                m_fpsCounter.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action, fps);
                m_lastUpdate = DateTime.Now;
            }
            fpsCounter++;
            stopwatch.Stop();

            //  Store the frame time.
            frameTime = stopwatch.Elapsed.TotalMilliseconds;
            return true;
        }

        private void Run()
        {
            run();
        }


        private void tickHandler(uint id, uint msg, ref int userCtx, int rsv1, int rsv2)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(Run));
        }

        // multimedia timer
        // https://www.codeproject.com/articles/17474/timer-surprises-and-how-to-avoid-them

        [DllImport("Winmm.dll")]
        private static extern int timeGetTime();

        [DllImport("winmm.dll")]
        private static extern uint timeGetDevCaps(out TimeCaps timeCaps, int size);


        struct TimeCaps
        {
            public uint minimum;
            public uint maximum;

            public TimeCaps(uint minimum, uint maximum)
            {
                this.minimum = minimum;
                this.maximum = maximum;
            }
        }

        [DllImport("WinMM.dll", SetLastError = true)]
        private static extern uint timeSetEvent(int msDelay, int msResolution,
                    TimerEventHandler handler, ref int userCtx, int eventType);

        [DllImport("WinMM.dll", SetLastError = true)]
        static extern uint timeKillEvent(uint timerEventId);

        public delegate void TimerEventHandler(uint id, uint msg, ref int userCtx,
            int rsv1, int rsv2);
    }
}
