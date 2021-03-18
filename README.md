# SmartConnector

![alt text](https://i.imgur.com/NMKAWMY.png)

## SECS/GEM Communication Interface
The SECS/GEM is the semiconductor's equipment interface protocol for equipment-to-host data communications. In an automated fab, the interface can start and stop equipment processing, collect measurement data, change variables and select recipes for products. The SECS (SEMI Equipment Communications Standard)/GEM (Generic Equipment Model) standards do all this in a defined way.

Developed by the SEMI (Semiconductor Equipment and Materials International) organization,[1] the standards define a common set of equipment behaviour and communications capabilities.

The Generic Model for Communications and Control Of Manufacturing Equipment (GEM) standard is maintained and published by the non-profit organization Semiconductor Equipment and Materials International (SEMI). Generally speaking, the SECS/GEM standard defines messages, state machines and scenarios to enable factory software to control and monitor manufacturing equipment. The GEM standard is formally designated and referred to as SEMI standard E30, but frequently simply referred to as the GEM or SECS/GEM standard. GEM intends "to produce economic benefits for both device manufacturers and equipment suppliers..." by defining "... a common set of equipment behavior and communications capabilities that provide the functionality and flexibility to support the manufacturing automation programs of semiconductor device manufacturers" [SEMI E30, 1.3]. GEM is a standard implementation of the SECS-II standard, SEMI standard E5. Many equipment in semiconductor (front end and back end), surface mount technology, electronics assembly, photovoltaic, flat panel display and other manufacturing industries worldwide provide a SECS/GEM interface on the manufacturing equipment so that the factory host software can communicate with the machine for monitoring and/or controlling purposes. Because the GEM standard was written with very few semiconductor-specific features, it can be applied to virtually any automated manufacturing equipment in any industry.

All GEM compliant manufacturing equipment share a consistent interface and certain consistent behavior. GEM equipment can communicate with a GEM capable host using either TCP/IP (using the HSMS standard, SEMI E37) or RS-232 based protocol (using the SECS-I standard, SEMI E4). Often both protocols are supported. Each equipment can be monitored and controlled using a common set of SECS-II messages specified by GEM.


## What is SmartConnector?
SmartConnector is a demo program used to communicate with the host computer through the SECS/GEM communication interface. SmartConnector is specifically made to communicate with Focussia's SecsQ program. SmartConnector relies on REST API to communicate with SecsQ sending and receiving messages in JSON. Information such as device IDs, lot IDs, event names, and alarm names can be communicated back and forth. SmartConnector was written in C# and the WPF framework loosely following the MVVM design pattern. Compiles in Visual Studio 2019 Community Edition. For educational purposes only. 

## Release Download (Run as Administrator)
https://drive.google.com/file/d/1ZmaHzg_MU_gZYbn9FHMmO3faso_7G7sg/view?usp=sharing
