local function OnPlayerConnecting(name, setKickReason, deferrals)
	deferrals.defer()
    deferrals.update('Girişiniz kontrol ediliyor...')
	
	identifiers = GetPlayerIdentifiers(source)
	local hex = identifiers[1]
	PerformHttpRequest('https://yalc.in/fivem_launcher/kontrol.php?steamid='..hex, function(err, text, headers) print(text) 
	local sonuc = text
	
	if sonuc == "-2" then
		deferrals.done('Sunucumuzda yasaklısınız.')
	elseif sonuc == "-1" then
		deferrals.done('Şu anda oyundasınız, tekrar giriş yapamazsınız.')
	elseif sonuc == "0" then
		deferrals.done('Sunucumuza girebilmek için launcher çalıştırmalısınız.')
	elseif sonuc == "1" then
		PerformHttpRequest('https://yalc.in/fivem_launcher/guncelle.php?steamid='..hex..'&durum=-1', function(err, text, headers) print(text) end, 'GET', '', { ["Content-Type"] = 'application/json' })
		deferrals.done()
	else
		deferrals.done('Whiteliste ekli değilsiniz.')
	end
end, 'GET', '', { ["Content-Type"] = 'application/json' })
end

local function OnPlayerDrop(name)
	local hex = identifiers[1]
	PerformHttpRequest('https://yalc.in/fivem_launcher/guncelle.php?steamid='..hex..'&durum=0', function(err, text, headers) print(text)end, 'GET', '', { ["Content-Type"] = 'application/json' }) 	
end

AddEventHandler("playerConnecting", OnPlayerConnecting)
AddEventHandler("playerDropped", OnPlayerDrop)