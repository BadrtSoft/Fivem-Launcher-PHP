local function OnPlayerConnecting(name, setKickReason, deferrals)
	deferrals.defer()
    deferrals.update('Girişiniz kontrol ediliyor...')
	
	identifiers = GetPlayerIdentifiers(source)
	local hex = identifiers[1]
	PerformHttpRequest('https://yalc.in/fivem_launcher/kontrol.php?steamid='..hex, function(err, text, headers) 
		if text == "-2" then
			deferrals.done('Sunucumuzda yasaklısınız.')
		elseif text == "-1" then
			deferrals.done('Şu anda oyundasınız, tekrar giriş yapamazsınız.')
		elseif text == "0" then
			deferrals.done('Sunucumuza girebilmek için launcher çalıştırmalısınız.')
		elseif text == "1" then
			PerformHttpRequest('https://yalc.in/fivem_launcher/guncelle.php?steamid='..hex..'&durum=-1', function(err, text, headers) print(text) end, 'GET', '', { ["Content-Type"] = 'application/json' })
			deferrals.done()
		else
			deferrals.done('Whiteliste ekli değilsiniz.')
		end
	end, 'GET', '', { ["Content-Type"] = 'application/json' })
end

local function OnPlayerDrop(name)
	local hex = identifiers[1]
	PerformHttpRequest('https://yalc.in/fivem_launcher/guncelle.php?steamid='..hex..'&durum=0', function(err, text, headers) print(text) end, 'GET', '', { ["Content-Type"] = 'application/json' }) 	
end

AddEventHandler("playerConnecting", OnPlayerConnecting)
AddEventHandler("playerDropped", OnPlayerDrop)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(30000)
		
		identifiers = GetPlayerIdentifiers(source)
		local hex = identifiers[1]
		PerformHttpRequest('https://yalc.in/fivem_launcher/kontrol.php?steamid='..hex, function(err, text, headers) 
			if text == "-2" then
				DropPlayer(source, 'Sunucumuzda yasaklısınız.')
			elseif text == "-1" then
				PerformHttpRequest('https://yalc.in/fivem_launcher/guncelle.php?steamid='..hex..'&durum=-1', function(err, text, headers) print(text) end, 'GET', '', { ["Content-Type"] = 'application/json' })
			elseif text == "0" then
				DropPlayer(source, 'Sunucumuza girebilmek için launcher çalıştırmalısınız.')
			elseif text == "1" then
				DropPlayer(source, 'Launcher kapatılmış.')
			else
				DropPlayer(source, 'Whiteliste ekli değilsiniz.')
			end
		end, 'GET', '', { ["Content-Type"] = 'application/json' })
	end
end)