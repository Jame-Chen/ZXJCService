installutil.exe ZXJCService.exe
Net Start RHPWKService
sc config RHPWKService start= auto
pause