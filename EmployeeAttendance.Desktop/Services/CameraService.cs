using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;

namespace EmployeeAttendance.Desktop.Services
{
    /// <summary>
    /// خدمة الكاميرا
    /// </summary>
    public class CameraService : IDisposable
    {
        // جهاز الكاميرا
        private FilterInfoCollection _videoDevices;
        
        // مصدر الفيديو
        private VideoCaptureDevice _videoSource;
        
        // الإطار الحالي
        private Bitmap _currentFrame;
        
        // مؤشر ما إذا كانت الكاميرا قيد التشغيل
        private bool _isRunning;
        
        // حدث التقاط إطار جديد
        public event EventHandler<Bitmap> NewFrameReceived;
        
        // حدث حدوث خطأ
        public event EventHandler<Exception> ErrorOccurred;

        public CameraService()
        {
            _isRunning = false;
        }

        /// <summary>
        /// الحصول على قائمة أجهزة الكاميرا المتاحة
        /// </summary>
        public FilterInfoCollection GetAvailableCameras()
        {
            try
            {
                _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                return _videoDevices;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }

        /// <summary>
        /// بدء تشغيل الكاميرا
        /// </summary>
        public void Start(int deviceIndex = 0)
        {
            try
            {
                if (_isRunning)
                {
                    Stop();
                }
                
                // التحقق من وجود أجهزة كاميرا
                if (_videoDevices == null || _videoDevices.Count == 0)
                {
                    _videoDevices = GetAvailableCameras();
                    
                    if (_videoDevices == null || _videoDevices.Count == 0)
                    {
                        throw new Exception("لا توجد أجهزة كاميرا متاحة");
                    }
                }
                
                // التحقق من صحة مؤشر الجهاز
                if (deviceIndex < 0 || deviceIndex >= _videoDevices.Count)
                {
                    deviceIndex = 0;
                }
                
                // إنشاء مصدر الفيديو
                _videoSource = new VideoCaptureDevice(_videoDevices[deviceIndex].MonikerString);
                
                // تعيين حدث التقاط إطار جديد
                _videoSource.NewFrame += VideoSource_NewFrame;
                
                // بدء تشغيل الكاميرا
                _videoSource.Start();
                
                _isRunning = true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        /// <summary>
        /// إيقاف تشغيل الكاميرا
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_videoSource != null && _videoSource.IsRunning)
                {
                    // إيقاف تشغيل الكاميرا
                    _videoSource.SignalToStop();
                    _videoSource.WaitForStop();
                    
                    // إزالة حدث التقاط إطار جديد
                    _videoSource.NewFrame -= VideoSource_NewFrame;
                    
                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        /// <summary>
        /// التقاط صورة
        /// </summary>
        public Bitmap CaptureImage()
        {
            try
            {
                if (_currentFrame != null)
                {
                    // إنشاء نسخة من الإطار الحالي
                    return (Bitmap)_currentFrame.Clone();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }

        /// <summary>
        /// التقاط صورة بشكل غير متزامن
        /// </summary>
        public async Task<Bitmap> CaptureImageAsync(int timeoutMilliseconds = 5000)
        {
            try
            {
                // إذا كانت الكاميرا غير قيد التشغيل، قم بتشغيلها
                if (!_isRunning)
                {
                    Start();
                    
                    // انتظار لحظة لبدء تشغيل الكاميرا
                    await Task.Delay(1000);
                }
                
                // إذا كان الإطار الحالي موجوداً، قم بإرجاعه
                if (_currentFrame != null)
                {
                    return (Bitmap)_currentFrame.Clone();
                }
                
                // انتظار التقاط إطار جديد
                using (var timeoutCancellationTokenSource = new CancellationTokenSource(timeoutMilliseconds))
                {
                    var tcs = new TaskCompletionSource<Bitmap>();
                    
                    // تسجيل حدث التقاط إطار جديد
                    EventHandler<Bitmap> handler = null;
                    handler = (sender, frame) =>
                    {
                        // إلغاء تسجيل الحدث
                        NewFrameReceived -= handler;
                        
                        // إكمال المهمة
                        tcs.TrySetResult((Bitmap)frame.Clone());
                    };
                    
                    NewFrameReceived += handler;
                    
                    // تسجيل إلغاء المهلة
                    timeoutCancellationTokenSource.Token.Register(() =>
                    {
                        // إلغاء تسجيل الحدث
                        NewFrameReceived -= handler;
                        
                        // إكمال المهمة بقيمة فارغة
                        tcs.TrySetResult(null);
                    });
                    
                    return await tcs.Task;
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return null;
            }
        }

        /// <summary>
        /// حدث التقاط إطار جديد
        /// </summary>
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // تحرير الإطار السابق
                if (_currentFrame != null)
                {
                    _currentFrame.Dispose();
                }
                
                // حفظ الإطار الحالي
                _currentFrame = (Bitmap)eventArgs.Frame.Clone();
                
                // إطلاق حدث التقاط إطار جديد
                OnNewFrameReceived(_currentFrame);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        /// <summary>
        /// إطلاق حدث التقاط إطار جديد
        /// </summary>
        protected virtual void OnNewFrameReceived(Bitmap frame)
        {
            NewFrameReceived?.Invoke(this, frame);
        }

        /// <summary>
        /// إطلاق حدث حدوث خطأ
        /// </summary>
        protected virtual void OnErrorOccurred(Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
        }

        /// <summary>
        /// التخلص من الموارد
        /// </summary>
        public void Dispose()
        {
            Stop();
            
            if (_currentFrame != null)
            {
                _currentFrame.Dispose();
                _currentFrame = null;
            }
        }
    }
}
