#pragma once

#include<AK/WwiseAuthoringAPI/AkAutobahn/Client.h>
#include<AK/WwiseAuthoringAPI/AkAutobahn/AkJson.h>

#include<AK/WwiseAuthoringAPI/AkVariantBase.h>

#include<stdio.h>
#include<string>

using namespace std;
using namespace AK::WwiseAuthoringAPI;



class Wwise
{
public:
	Wwise() {};
	~Wwise() {};

	bool TryConnect(const string& IpAddress, const string& PortNumber);

	bool Set(const AkJson& ObjectsArgs, const AkJson& ReturnOptionArgs, AkJson& ReturnArgs);

	bool GetByWaql(const string& Waql, const AkJson& ReturnOptionArgs, AkJson& ReturnArgs);

private:
	Client ClientInstance;
	bool ConnectState = false;
};

