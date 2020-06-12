Citizen.CreateThread(function()
	Citizen.Wait(60000)
	while true do
		TriggerServerEvent('LauncherKontrol:checkPlayer', PlayerId())
		Citizen.Wait(60000)
	end
end)