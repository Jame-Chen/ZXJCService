installutil.exe ZXJCService.exe
Net Start MHZHSWService
sc config MHZHSWService start= auto
pause