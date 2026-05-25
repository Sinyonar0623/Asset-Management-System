[CmdletBinding()]
param(
    [string]$ApiBaseUrl = "http://localhost:5176",
    [string]$AccessToken,
    [switch]$ContinueOnError
)

$ErrorActionPreference = "Stop"

$baseUrl = $ApiBaseUrl.TrimEnd("/")
$assetUnitEndpoint = "$baseUrl/AssetUnit"
$assetEndpoint = "$baseUrl/Asset"

$headers = @{}
if (-not [string]::IsNullOrWhiteSpace($AccessToken)) {
    $headers["Authorization"] = "Bearer $AccessToken"
}

$assetUnits = @(
    @{
        key = "nb-office-01-laptop"
        assetTag = "AMS-NB-001"
        serialNo = "NB2026-001"
        name = "Office Notebook 01"
        brand = "Lenovo"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed notebook unit for office bundle 1."
        responsibleUserId = $null
    },
    @{
        key = "nb-office-01-monitor"
        assetTag = "AMS-MON-001"
        serialNo = "MON2026-001"
        name = "Office Monitor 01"
        brand = "Dell"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed monitor unit for office bundle 1."
        responsibleUserId = $null
    },
    @{
        key = "nb-office-01-charger"
        assetTag = "AMS-CHG-001"
        serialNo = "CHG2026-001"
        name = "Notebook Charger 01"
        brand = "Lenovo"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed charger unit for office bundle 1."
        responsibleUserId = $null
    },
    @{
        key = "nb-office-02-laptop"
        assetTag = "AMS-NB-002"
        serialNo = "NB2026-002"
        name = "Office Notebook 02"
        brand = "HP"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed notebook unit for office bundle 2."
        responsibleUserId = $null
    },
    @{
        key = "nb-office-02-monitor"
        assetTag = "AMS-MON-002"
        serialNo = "MON2026-002"
        name = "Office Monitor 02"
        brand = "HP"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed monitor unit for office bundle 2."
        responsibleUserId = $null
    },
    @{
        key = "nb-office-02-charger"
        assetTag = "AMS-CHG-002"
        serialNo = "CHG2026-002"
        name = "Notebook Charger 02"
        brand = "HP"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed charger unit for office bundle 2."
        responsibleUserId = $null
    },
    @{
        key = "aio-01-cpu"
        assetTag = "AMS-AIO-001"
        serialNo = "AIO2026-001"
        name = "All-in-One Workstation 01"
        brand = "Dell"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed all-in-one workstation."
        responsibleUserId = $null
    },
    @{
        key = "aio-01-keyboard"
        assetTag = "AMS-KBD-001"
        serialNo = "KBD2026-001"
        name = "Keyboard 01"
        brand = "Logitech"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed keyboard unit for workstation."
        responsibleUserId = $null
    },
    @{
        key = "aio-01-mouse"
        assetTag = "AMS-MSE-001"
        serialNo = "MSE2026-001"
        name = "Mouse 01"
        brand = "Logitech"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed mouse unit for workstation."
        responsibleUserId = $null
    },
    @{
        key = "iot-router-01"
        assetTag = "AMS-IOT-RTR-001"
        serialNo = "IOTRTR2026-001"
        name = "IoT Router 01"
        brand = "Cisco"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed IoT router unit."
        responsibleUserId = $null
    },
    @{
        key = "iot-switch-01"
        assetTag = "AMS-IOT-SW-001"
        serialNo = "IOTSW2026-001"
        name = "IoT Switch 01"
        brand = "TP-Link"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed IoT switch unit."
        responsibleUserId = $null
    },
    @{
        key = "iot-gateway-01"
        assetTag = "AMS-IOT-GW-001"
        serialNo = "IOTGW2026-001"
        name = "IoT Gateway 01"
        brand = "Raspberry Pi"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed IoT gateway unit."
        responsibleUserId = $null
    },
    @{
        key = "dev-esp32-01"
        assetTag = "AMS-DEV-ESP32-001"
        serialNo = "ESP322026-001"
        name = "ESP32 Development Board 01"
        brand = "Espressif"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed embedded development board."
        responsibleUserId = $null
    },
    @{
        key = "dev-arduino-01"
        assetTag = "AMS-DEV-ARD-001"
        serialNo = "ARD2026-001"
        name = "Arduino Development Board 01"
        brand = "Arduino"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed embedded development board."
        responsibleUserId = $null
    },
    @{
        key = "dev-raspi-01"
        assetTag = "AMS-DEV-RPI-001"
        serialNo = "RPI2026-001"
        name = "Raspberry Pi Board 01"
        brand = "Raspberry Pi"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Seed embedded development board."
        responsibleUserId = $null
    },
    @{
        key = "spare-projector-01"
        assetTag = "AMS-SPR-PROJ-001"
        serialNo = "PROJ2026-001"
        name = "Spare Projector 01"
        brand = "Epson"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Intentionally left unassigned for unit assignment testing."
        responsibleUserId = $null
    },
    @{
        key = "spare-camera-01"
        assetTag = "AMS-SPR-CAM-001"
        serialNo = "CAM2026-001"
        name = "Spare Document Camera 01"
        brand = "Aver"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Intentionally left unassigned for unit assignment testing."
        responsibleUserId = $null
    },
    @{
        key = "spare-tablet-01"
        assetTag = "AMS-SPR-TAB-001"
        serialNo = "TAB2026-001"
        name = "Spare Tablet 01"
        brand = "Samsung"
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Intentionally left unassigned for unit assignment testing."
        responsibleUserId = $null
    }
)

$assets = @(
    @{
        asset = @{
            name = "Office Notebook Bundle 1"
            description = "Notebook bundle with monitor and charger for office use."
            category = "NOTEBOOK_SET"
        }
        unitKeys = @("nb-office-01-laptop", "nb-office-01-monitor", "nb-office-01-charger")
    },
    @{
        asset = @{
            name = "Office Notebook Bundle 2"
            description = "Notebook bundle with monitor and charger for office use."
            category = "NOTEBOOK_SET"
        }
        unitKeys = @("nb-office-02-laptop", "nb-office-02-monitor", "nb-office-02-charger")
    },
    @{
        asset = @{
            name = "All-in-One Workstation 1"
            description = "All-in-one desktop with dedicated mouse and keyboard."
            category = "AIO_PC"
        }
        unitKeys = @("aio-01-cpu", "aio-01-keyboard", "aio-01-mouse")
    },
    @{
        asset = @{
            name = "IoT Network Kit 1"
            description = "IoT infrastructure set with router, switch, and gateway devices."
            category = "IOT_KIT"
        }
        unitKeys = @("iot-router-01", "iot-switch-01", "iot-gateway-01")
    },
    @{
        asset = @{
            name = "Embedded Development Kit 1"
            description = "Development kit with ESP32, Arduino, and Raspberry Pi boards."
            category = "MCU_KIT"
        }
        unitKeys = @("dev-esp32-01", "dev-arduino-01", "dev-raspi-01")
    }
)

foreach ($number in 3..14) {
    $suffix = "{0:D2}" -f $number
    $notebookBrand = if ($number % 2 -eq 0) { "HP" } else { "Lenovo" }
    $monitorBrand = if ($number % 2 -eq 0) { "HP" } else { "Dell" }

    $assetUnits += @(
        @{
            key = "nb-office-$suffix-laptop"
            assetTag = "AMS-NB-$suffix"
            serialNo = "NB2026-$suffix"
            name = "Office Notebook $suffix"
            brand = $notebookBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed notebook unit for office bundle $suffix."
            responsibleUserId = $null
        },
        @{
            key = "nb-office-$suffix-monitor"
            assetTag = "AMS-MON-$suffix"
            serialNo = "MON2026-$suffix"
            name = "Office Monitor $suffix"
            brand = $monitorBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed monitor unit for office bundle $suffix."
            responsibleUserId = $null
        },
        @{
            key = "nb-office-$suffix-charger"
            assetTag = "AMS-CHG-$suffix"
            serialNo = "CHG2026-$suffix"
            name = "Notebook Charger $suffix"
            brand = $notebookBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed charger unit for office bundle $suffix."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Office Notebook Bundle $number"
            description = "Generated notebook bundle with monitor and charger for office and classroom use."
            category = "NOTEBOOK_SET"
        }
        unitKeys = @("nb-office-$suffix-laptop", "nb-office-$suffix-monitor", "nb-office-$suffix-charger")
    }
}

foreach ($number in 2..10) {
    $suffix = "{0:D2}" -f $number
    $workstationBrand = if ($number % 2 -eq 0) { "Dell" } else { "Acer" }

    $assetUnits += @(
        @{
            key = "aio-$suffix-cpu"
            assetTag = "AMS-AIO-$suffix"
            serialNo = "AIO2026-$suffix"
            name = "All-in-One Workstation $suffix"
            brand = $workstationBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed all-in-one workstation."
            responsibleUserId = $null
        },
        @{
            key = "aio-$suffix-keyboard"
            assetTag = "AMS-KBD-$suffix"
            serialNo = "KBD2026-$suffix"
            name = "Keyboard $suffix"
            brand = "Logitech"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed keyboard unit for workstation."
            responsibleUserId = $null
        },
        @{
            key = "aio-$suffix-mouse"
            assetTag = "AMS-MSE-$suffix"
            serialNo = "MSE2026-$suffix"
            name = "Mouse $suffix"
            brand = "Logitech"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed mouse unit for workstation."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "All-in-One Workstation $number"
            description = "Generated all-in-one desktop with dedicated mouse and keyboard."
            category = "AIO_PC"
        }
        unitKeys = @("aio-$suffix-cpu", "aio-$suffix-keyboard", "aio-$suffix-mouse")
    }
}

foreach ($number in 2..8) {
    $suffix = "{0:D2}" -f $number
    $assetUnits += @(
        @{
            key = "iot-router-$suffix"
            assetTag = "AMS-IOT-RTR-$suffix"
            serialNo = "IOTRTR2026-$suffix"
            name = "IoT Router $suffix"
            brand = "Cisco"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed IoT router unit."
            responsibleUserId = $null
        },
        @{
            key = "iot-switch-$suffix"
            assetTag = "AMS-IOT-SW-$suffix"
            serialNo = "IOTSW2026-$suffix"
            name = "IoT Switch $suffix"
            brand = "TP-Link"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed IoT switch unit."
            responsibleUserId = $null
        },
        @{
            key = "iot-gateway-$suffix"
            assetTag = "AMS-IOT-GW-$suffix"
            serialNo = "IOTGW2026-$suffix"
            name = "IoT Gateway $suffix"
            brand = "Raspberry Pi"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed IoT gateway unit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "IoT Network Kit $number"
            description = "Generated IoT infrastructure set with router, switch, and gateway devices."
            category = "IOT_KIT"
        }
        unitKeys = @("iot-router-$suffix", "iot-switch-$suffix", "iot-gateway-$suffix")
    }
}

foreach ($number in 2..8) {
    $suffix = "{0:D2}" -f $number
    $assetUnits += @(
        @{
            key = "dev-esp32-$suffix"
            assetTag = "AMS-DEV-ESP32-$suffix"
            serialNo = "ESP322026-$suffix"
            name = "ESP32 Development Board $suffix"
            brand = "Espressif"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed embedded development board."
            responsibleUserId = $null
        },
        @{
            key = "dev-arduino-$suffix"
            assetTag = "AMS-DEV-ARD-$suffix"
            serialNo = "ARD2026-$suffix"
            name = "Arduino Development Board $suffix"
            brand = "Arduino"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed embedded development board."
            responsibleUserId = $null
        },
        @{
            key = "dev-raspi-$suffix"
            assetTag = "AMS-DEV-RPI-$suffix"
            serialNo = "RPI2026-$suffix"
            name = "Raspberry Pi Board $suffix"
            brand = "Raspberry Pi"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated seed embedded development board."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Embedded Development Kit $number"
            description = "Generated development kit with ESP32, Arduino, and Raspberry Pi boards."
            category = "MCU_KIT"
        }
        unitKeys = @("dev-esp32-$suffix", "dev-arduino-$suffix", "dev-raspi-$suffix")
    }
}

foreach ($number in 2..15) {
    $suffix = "{0:D2}" -f $number
    $spareBrand = switch ($number % 3) {
        0 { "Epson" }
        1 { "Samsung" }
        default { "Aver" }
    }

    $assetUnits += @{
        key = "spare-lab-$suffix"
        assetTag = "AMS-SPR-LAB-$suffix"
        serialNo = "SPARE2026-$suffix"
        name = "Spare Lab Equipment $suffix"
        brand = $spareBrand
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Generated unassigned seed unit for assignment testing."
        responsibleUserId = $null
    }
}

foreach ($number in 1..10) {
    $suffix = "{0:D2}" -f $number
    $projectorBrand = if ($number % 2 -eq 0) { "Epson" } else { "BenQ" }

    $assetUnits += @(
        @{
            key = "projector-kit-$suffix-projector"
            assetTag = "AMS-PRJ-$suffix"
            serialNo = "PRJ2026-$suffix"
            name = "Classroom Projector $suffix"
            brand = $projectorBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated projector unit for classroom presentation kit."
            responsibleUserId = $null
        },
        @{
            key = "projector-kit-$suffix-screen"
            assetTag = "AMS-SCR-$suffix"
            serialNo = "SCR2026-$suffix"
            name = "Portable Projection Screen $suffix"
            brand = "Elite Screens"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated projection screen unit for classroom presentation kit."
            responsibleUserId = $null
        },
        @{
            key = "projector-kit-$suffix-hdmi"
            assetTag = "AMS-HDMI-$suffix"
            serialNo = "HDMI2026-$suffix"
            name = "HDMI Adapter Set $suffix"
            brand = "UGreen"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated adapter unit for classroom presentation kit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Classroom Presentation Kit $number"
            description = "Generated classroom presentation set with projector, portable screen, and HDMI adapter."
            category = "PRESENTATION_KIT"
        }
        unitKeys = @("projector-kit-$suffix-projector", "projector-kit-$suffix-screen", "projector-kit-$suffix-hdmi")
    }
}

foreach ($number in 1..12) {
    $suffix = "{0:D2}" -f $number
    $tabletBrand = if ($number % 2 -eq 0) { "Samsung" } else { "Lenovo" }

    $assetUnits += @(
        @{
            key = "tablet-kit-$suffix-tablet-a"
            assetTag = "AMS-TAB-$suffix-A"
            serialNo = "TAB2026-$suffix-A"
            name = "Student Tablet $suffix A"
            brand = $tabletBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated tablet unit for mobile lab kit."
            responsibleUserId = $null
        },
        @{
            key = "tablet-kit-$suffix-tablet-b"
            assetTag = "AMS-TAB-$suffix-B"
            serialNo = "TAB2026-$suffix-B"
            name = "Student Tablet $suffix B"
            brand = $tabletBrand
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated tablet unit for mobile lab kit."
            responsibleUserId = $null
        },
        @{
            key = "tablet-kit-$suffix-charger"
            assetTag = "AMS-TCHG-$suffix"
            serialNo = "TCHG2026-$suffix"
            name = "Tablet Charging Dock $suffix"
            brand = "Anker"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated charging dock unit for mobile lab kit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Mobile Tablet Lab Kit $number"
            description = "Generated mobile lab set with two student tablets and one charging dock."
            category = "MOBILE_LAB_KIT"
        }
        unitKeys = @("tablet-kit-$suffix-tablet-a", "tablet-kit-$suffix-tablet-b", "tablet-kit-$suffix-charger")
    }
}

foreach ($number in 1..10) {
    $suffix = "{0:D2}" -f $number

    $assetUnits += @(
        @{
            key = "robotics-kit-$suffix-controller"
            assetTag = "AMS-ROB-CTRL-$suffix"
            serialNo = "ROBCTRL2026-$suffix"
            name = "Robotics Controller $suffix"
            brand = "Arduino"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated robotics controller unit."
            responsibleUserId = $null
        },
        @{
            key = "robotics-kit-$suffix-sensor"
            assetTag = "AMS-ROB-SEN-$suffix"
            serialNo = "ROBSEN2026-$suffix"
            name = "Robotics Sensor Pack $suffix"
            brand = "DFRobot"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated robotics sensor pack unit."
            responsibleUserId = $null
        },
        @{
            key = "robotics-kit-$suffix-motor"
            assetTag = "AMS-ROB-MTR-$suffix"
            serialNo = "ROBMTR2026-$suffix"
            name = "Robotics Motor Pack $suffix"
            brand = "Pololu"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated robotics motor pack unit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Robotics Practice Kit $number"
            description = "Generated robotics practice set with controller, sensor pack, and motor pack."
            category = "ROBOTICS_EQUIPMENT"
        }
        unitKeys = @("robotics-kit-$suffix-controller", "robotics-kit-$suffix-sensor", "robotics-kit-$suffix-motor")
    }
}

foreach ($number in 1..8) {
    $suffix = "{0:D2}" -f $number

    $assetUnits += @(
        @{
            key = "measurement-kit-$suffix-scope"
            assetTag = "AMS-MEA-OSC-$suffix"
            serialNo = "OSC2026-$suffix"
            name = "Digital Oscilloscope $suffix"
            brand = "Rigol"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated oscilloscope unit for measurement kit."
            responsibleUserId = $null
        },
        @{
            key = "measurement-kit-$suffix-meter"
            assetTag = "AMS-MEA-MTR-$suffix"
            serialNo = "MTR2026-$suffix"
            name = "Digital Multimeter $suffix"
            brand = "Fluke"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated multimeter unit for measurement kit."
            responsibleUserId = $null
        },
        @{
            key = "measurement-kit-$suffix-supply"
            assetTag = "AMS-MEA-PSU-$suffix"
            serialNo = "PSU2026-$suffix"
            name = "Bench Power Supply $suffix"
            brand = "GW Instek"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated power supply unit for measurement kit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Electronics Measurement Kit $number"
            description = "Generated electronics measurement set with oscilloscope, multimeter, and power supply."
            category = "MEASUREMENT_KIT"
        }
        unitKeys = @("measurement-kit-$suffix-scope", "measurement-kit-$suffix-meter", "measurement-kit-$suffix-supply")
    }
}

foreach ($number in 1..8) {
    $suffix = "{0:D2}" -f $number

    $assetUnits += @(
        @{
            key = "network-kit-$suffix-router"
            assetTag = "AMS-NET-RTR-$suffix"
            serialNo = "NETRTR2026-$suffix"
            name = "Network Lab Router $suffix"
            brand = "Cisco"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated router unit for network lab kit."
            responsibleUserId = $null
        },
        @{
            key = "network-kit-$suffix-switch"
            assetTag = "AMS-NET-SW-$suffix"
            serialNo = "NETSW2026-$suffix"
            name = "Network Lab Switch $suffix"
            brand = "Cisco"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated switch unit for network lab kit."
            responsibleUserId = $null
        },
        @{
            key = "network-kit-$suffix-ap"
            assetTag = "AMS-NET-AP-$suffix"
            serialNo = "NETAP2026-$suffix"
            name = "Wireless Access Point $suffix"
            brand = "Ubiquiti"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated access point unit for network lab kit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "Network Laboratory Kit $number"
            description = "Generated network laboratory set with router, switch, and wireless access point."
            category = "NETWORK_LAB_KIT"
        }
        unitKeys = @("network-kit-$suffix-router", "network-kit-$suffix-switch", "network-kit-$suffix-ap")
    }
}

foreach ($number in 1..6) {
    $suffix = "{0:D2}" -f $number

    $assetUnits += @(
        @{
            key = "vr-kit-$suffix-headset"
            assetTag = "AMS-VR-HMD-$suffix"
            serialNo = "VRHMD2026-$suffix"
            name = "VR Headset $suffix"
            brand = "Meta"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated VR headset unit."
            responsibleUserId = $null
        },
        @{
            key = "vr-kit-$suffix-left-controller"
            assetTag = "AMS-VR-LC-$suffix"
            serialNo = "VRLC2026-$suffix"
            name = "VR Left Controller $suffix"
            brand = "Meta"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated VR left controller unit."
            responsibleUserId = $null
        },
        @{
            key = "vr-kit-$suffix-right-controller"
            assetTag = "AMS-VR-RC-$suffix"
            serialNo = "VRRC2026-$suffix"
            name = "VR Right Controller $suffix"
            brand = "Meta"
            availabilityStatus = "PENDING_ACTIVATION"
            operationalStatus = "READY"
            remark = "Generated VR right controller unit."
            responsibleUserId = $null
        }
    )

    $assets += @{
        asset = @{
            name = "VR Learning Kit $number"
            description = "Generated VR learning set with headset and paired controllers."
            category = "VR_KIT"
        }
        unitKeys = @("vr-kit-$suffix-headset", "vr-kit-$suffix-left-controller", "vr-kit-$suffix-right-controller")
    }
}

foreach ($number in 16..35) {
    $suffix = "{0:D2}" -f $number
    $spareBrand = switch ($number % 4) {
        0 { "Logitech" }
        1 { "Sony" }
        2 { "Belkin" }
        default { "Kingston" }
    }

    $assetUnits += @{
        key = "spare-mixed-$suffix"
        assetTag = "AMS-SPR-MIX-$suffix"
        serialNo = "MIXSPARE2026-$suffix"
        name = "Mixed Spare Equipment $suffix"
        brand = $spareBrand
        availabilityStatus = "PENDING_ACTIVATION"
        operationalStatus = "READY"
        remark = "Generated extra unassigned seed unit for assignment testing."
        responsibleUserId = $null
    }
}

Write-Host "POST $assetUnitEndpoint"
Write-Host "Creating $($assetUnits.Count) asset units"

$assetUnitDtos = foreach ($unit in $assetUnits) {
    @{
        assetTag = $unit.assetTag
        serialNo = $unit.serialNo
        name = $unit.name
        brand = $unit.brand
        availabilityStatus = $unit.availabilityStatus
        operationalStatus = $unit.operationalStatus
        remark = $unit.remark
        responsibleUserId = $unit.responsibleUserId
    }
}

$assetUnitPayload = @{
    assetUnits = @($assetUnitDtos)
} | ConvertTo-Json -Depth 10

$createdUnitResponse = Invoke-RestMethod `
    -Method Post `
    -Uri $assetUnitEndpoint `
    -Headers $headers `
    -ContentType "application/json" `
    -Body $assetUnitPayload

$createdUnitIds = @($createdUnitResponse.id)
if ($createdUnitIds.Count -ne $assetUnits.Count) {
    throw "Create AssetUnit returned $($createdUnitIds.Count) ids, expected $($assetUnits.Count)."
}

$unitIdsByKey = @{}
for ($i = 0; $i -lt $assetUnits.Count; $i++) {
    $unitIdsByKey[$assetUnits[$i].key] = $createdUnitIds[$i]
}

$assetResults = foreach ($item in $assets) {
    $name = $item.asset.name
    $unitIds = foreach ($key in $item.unitKeys) {
        if (-not $unitIdsByKey.ContainsKey($key)) {
            throw "Unknown unit key '$key' for asset '$name'."
        }

        $unitIdsByKey[$key]
    }

    $body = @{
        asset = $item.asset
        units = @($unitIds)
    } | ConvertTo-Json -Depth 10

    Write-Host "POST $assetEndpoint"
    Write-Host "Creating asset: $name with $($unitIds.Count) units"

    try {
        $response = Invoke-RestMethod `
            -Method Post `
            -Uri $assetEndpoint `
            -Headers $headers `
            -ContentType "application/json" `
            -Body $body

        [PSCustomObject]@{
            Name = $name
            UnitCount = $unitIds.Count
            Succeeded = $true
            Response = $response
            Error = $null
        }
    }
    catch {
        $errorMessage = $_.Exception.Message

        if (-not $ContinueOnError) {
            throw
        }

        [PSCustomObject]@{
            Name = $name
            UnitCount = $unitIds.Count
            Succeeded = $false
            Response = $null
            Error = $errorMessage
        }
    }
}

$assignedUnitKeys = @($assets | ForEach-Object { $_.unitKeys } | ForEach-Object { $_ })
$unassignedUnits = $assetUnits |
    Where-Object { $assignedUnitKeys -notcontains $_.key } |
    ForEach-Object {
        [PSCustomObject]@{
            Key = $_.key
            AssetTag = $_.assetTag
            Name = $_.name
            Id = $unitIdsByKey[$_.key]
        }
    }

Write-Host ""
Write-Host "Created $($createdUnitIds.Count) asset units."
Write-Host "Created $($assetResults.Count) assets."
Write-Host "Left $($unassignedUnits.Count) asset units unassigned."

[PSCustomObject]@{
    CreatedAssetUnits = $createdUnitIds.Count
    CreatedAssets = $assetResults
    UnassignedAssetUnits = $unassignedUnits
}
