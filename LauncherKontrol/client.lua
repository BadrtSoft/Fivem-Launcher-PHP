Citizen.CreateThread(function()
	Citizen.Wait(5000)
	while true do
		TriggerServerEvent('LauncherKontrol:checkPlayer', PlayerId())
		Citizen.Wait(5000)
	end
end)