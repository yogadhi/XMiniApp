using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XMiniApp.Models;
using Newtonsoft.Json;
using XMiniApp.Controllers;
using XMiniApp.DependencyServices;

namespace XMiniApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        System.Timers.Timer timer;

        Color _ProgressColor = Color.LightGreen;
        public Color ProgressColor
        {
            get { return _ProgressColor; }
            set
            {
                _ProgressColor = value;
                OnPropertyChanged();
            }
        }

        decimal _MaxInterval = 30m;
        public decimal MaxInterval
        {
            get { return _MaxInterval; }
            set
            {
                _MaxInterval = value;
                OnPropertyChanged();
            }
        }

        TimeSpan _ts;
        public TimeSpan ts
        {
            get { return _ts; }
            set
            {
                _ts = value;
                OnPropertyChanged();
            }
        }

        DateTime _StartTime;
        public DateTime StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                OnPropertyChanged();
            }
        }

        DateTime _EndTime;
        public DateTime EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                OnPropertyChanged();
            }
        }

        string _CurrentOTP = string.Empty;
        public string CurrentOTP
        {
            get { return _CurrentOTP; }
            set
            {
                _CurrentOTP = value;
                OnPropertyChanged();
            }
        }

        string _CTimer = string.Empty;
        public string CTimer
        {
            get { return _CTimer; }
            set
            {
                _CTimer = value;
                OnPropertyChanged();
            }
        }

        string _Progress;
        public string Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenWebCommand { get; }

        public AboutViewModel()
        {
            Title = "Time-Based OTP";
            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            //LoadCommand.Execute(null);
        }

        ICommand _LoadCommand;
        public ICommand LoadCommand => _LoadCommand ?? (_LoadCommand = new Command(async () => await ExecuteLoadCommand()));
        async Task ExecuteLoadCommand()
        {
            IsBusy = true;
            await Task.Delay(3000);

            try
            {
                var resOTP = await GenerateTOTP();
                if (resOTP != null)
                {
                    if (!string.IsNullOrEmpty(resOTP.TOTP))
                    {
                        StartTime = DateTime.Now;
                        EndTime = StartTime.AddSeconds(30);
                        var tsp = EndTime - StartTime;
                        MaxInterval = tsp.Seconds;
                        CurrentOTP = resOTP.TOTP;

                        ts = EndTime - DateTime.Now;
                        CTimer = ts.Seconds.ToString();
                    }
                }

                timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += t_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            try
            {
                ts = EndTime - DateTime.Now;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Code to run on the main thread  
                    var divideVal = (decimal)ts.Seconds / (decimal)MaxInterval;
                    var roundDivideVal = Math.Round(divideVal, 2, MidpointRounding.AwayFromZero);
                    Progress = roundDivideVal.ToString();
                    CTimer = ts.Seconds.ToString();


                });

                if (1 <= ts.TotalSeconds && ts.TotalSeconds <= 5)
                {
                    ProgressColor = Color.Red;
                }
                else if (6 <= ts.TotalSeconds && ts.TotalSeconds <= 15)
                {
                    ProgressColor = Color.Yellow;
                }
                else if (ts.TotalSeconds >= 16)
                {
                    ProgressColor = Color.LightGreen;
                }

                if (ts.TotalSeconds <= 1)
                {
                    timer.Stop();
                    LoadCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        async Task<GenerateOTPRes> GenerateTOTP()
        {
            GenerateOTPRes resOTP = new GenerateOTPRes();

            try
            {
                var strToken = await APIAgent.GenerateToken();
                //strToken = "vIu76Vsh_wLRg_51npQwDWv4MUA20OMQSFteFvOQfDrs6fmbN3en2iHjqzYiuH2neQPn6RtFgCPPzHZTBrHcadt4nBx9LXkIIjuLQkVgejkqdjnaNz5BfrgxviAKc6uN-LU4MKKCkkIPEvcb8VznGbD7ukw2";

                if (!string.IsNullOrEmpty(strToken))
                {
                    var objOTPAPI = await APIAgent.AccessAPI(APIAgent.APIEnum.POST, 3, "MiniAPI/GenerateOTP", strToken, "");
                    if (!string.IsNullOrEmpty(objOTPAPI))
                    {
                        resOTP = JsonConvert.DeserializeObject<GenerateOTPRes>(objOTPAPI);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return resOTP;
        }

        ICommand _CopyClipboardCommand;
        public ICommand CopyClipboardCommand => _CopyClipboardCommand ?? (_CopyClipboardCommand = new Command(async () => await ExecuteCopyClipboardCommand()));
        async Task ExecuteCopyClipboardCommand()
        {
            try
            {
                await Clipboard.SetTextAsync(CurrentOTP);
                DependencyService.Get<IDeviceLinkedService>().LongAlert(CurrentOTP + " copied to clipboard");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}