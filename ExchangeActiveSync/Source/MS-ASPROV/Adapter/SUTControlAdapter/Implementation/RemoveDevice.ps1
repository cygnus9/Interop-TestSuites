#-------------------------------------------------------------------------
# Copyright (c) 2014 Microsoft Corporation. All rights reserved.
# Use of this sample source code is subject to the terms of the Microsoft license 
# agreement under which you licensed this sample source code and is provided AS-IS.
# If you did not accept the terms of the license agreement, you are not authorized 
# to use this sample source code. For the terms of the license, please see the 
# license agreement between you and Microsoft.
#-------------------------------------------------------------------------

$script:ErrorActionPreference = "Stop"
$adminDomain = $ptfpropDomain
$adminName = $ptfpropUser1Name
$adminPassword = $ptfpropUser1Password
$sutVersion = $ptfpropSutVersion

$adminAccount = $adminDomain + "\" + $adminName
$deviceWiped = $false

if($sutVersion -ge "ExchangeServer2010")
{
  $securePassword = ConvertTo-SecureString $adminPassword -AsPlainText -Force
  $credential = new-object Management.Automation.PSCredential($adminAccount,$securePassword)

  #Invoke function remotely
  $ret = invoke-command -computer $serverComputerName -Credential $credential -scriptblock {
	  param(
	       [string]$userEmail,
		   [System.Object]$credential,
		   [string]$serverComputerName,
		   [string]$deviceType
	  )
	   
	  $connectUri = "http://" + $serverComputerName + "/PowerShell"
	  $session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri $connectUri -Credential $credential -Authentication Kerberos
	  Import-PSSession $session -AllowClobber -DisableNameChecking

	  #Remove the specified device
	  $devices = Get-ActiveSyncDeviceStatistics -Mailbox $userEmail
	  
	  foreach($device in $devices)
	  {
		if($device.DeviceType -eq $deviceType)
		{
			Remove-ActiveSyncDevice -Identity $device.Identity -Confirm:$false
		}
	  }

	  return $true
  }-ArgumentList $userEmail,$credential,$serverComputerName,$deviceType

  return $ret
}
else
{
  cmd /c "winrs -r:$serverComputerName -u:$adminAccount -p:$adminPassword Powershell Add-PSSnapin Microsoft.Exchange.Management.PowerShell.Admin;`$devices = Get-ActiveSyncDeviceStatistics -Mailbox $userEmail;foreach(`$device in `$devices){if(`$device.DeviceType -eq '$deviceType'){ Remove-ActiveSyncDevice -Identity `$device.Identity -Confirm:`$false }}"
  return $true
}