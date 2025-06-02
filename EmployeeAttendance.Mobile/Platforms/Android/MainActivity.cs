using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeAttendance.Mobile
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
        ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class MainActivity : MauiAppCompatActivity
    {
        // Permission request code
        private const int RequestPermissionsCode = 1000;

        // List of required permissions
        private string[] GetRequiredPermissions()
        {
            var permissions = new List<string>
            {
                Android.Manifest.Permission.Camera,
                Android.Manifest.Permission.Internet,
                Android.Manifest.Permission.AccessNetworkState
            };

            // إضافة أذونات التخزين بناءً على إصدار Android
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
            {
                permissions.Add(Android.Manifest.Permission.ReadMediaImages);
            }
            else
            {
                permissions.Add(Android.Manifest.Permission.ReadExternalStorage);
                permissions.Add(Android.Manifest.Permission.WriteExternalStorage);
            }

            // إضافة أذونات تثبيت التطبيقات بناءً على إصدار Android
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M) // Android 6.0+
            {
                permissions.Add(Android.Manifest.Permission.RequestInstallPackages);
            }

            return permissions.ToArray();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // تهيئة المنصة
                Platform.Init(this, savedInstanceState);

                // تأخير طلب الأذونات لتجنب الأخطاء عند بدء التشغيل
                Task.Run(async () =>
                {
                    try
                    {
                        // تأخير أطول لضمان اكتمال تهيئة التطبيق
                        await Task.Delay(3000);

                        // طلب الأذونات على الواجهة الرئيسية
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                // طلب الأذونات المطلوبة
                                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                                {
                                    RequestPermissions(GetRequiredPermissions(), RequestPermissionsCode);
                                    System.Diagnostics.Debug.WriteLine("Permissions requested successfully");
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error requesting permissions: {ex.Message}");
                                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in Task.Run: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnCreate: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestPermissionsCode)
            {
                // Check if all permissions are granted
                bool allPermissionsGranted = true;
                for (int i = 0; i < grantResults.Length; i++)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        allPermissionsGranted = false;
                        break;
                    }
                }

                if (!allPermissionsGranted)
                {
                    // Show alert dialog if permissions are not granted
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Permissions Required")
                           .SetMessage("This app requires certain permissions to function properly. Please grant all requested permissions.")
                           .SetPositiveButton("OK", (sender, args) =>
                           {
                               // Request permissions again
                               RequestPermissions(GetRequiredPermissions(), RequestPermissionsCode);
                           })
                           .SetNegativeButton("Cancel", (sender, args) =>
                           {
                               // Close the app if permissions are denied
                               Finish();
                           })
                           .Show();
                }
            }
        }

        // Handle installation of APK files
        public static void InstallApk(Context context, string apkFilePath)
        {
            try
            {
                Java.IO.File file = new Java.IO.File(apkFilePath);

                Intent intent = new Intent(Intent.ActionView);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    // For Android 7.0 and above, we need to use FileProvider
                    Android.Net.Uri apkUri = AndroidX.Core.Content.FileProvider.GetUriForFile(
                        context,
                        context.PackageName + ".fileprovider",
                        file);

                    intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                }
                else
                {
                    // For older versions
                    intent.SetDataAndType(Android.Net.Uri.FromFile(file), "application/vnd.android.package-archive");
                }

                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error installing APK: {ex.Message}");
            }
        }
    }
}
