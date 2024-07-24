#include "Wwise.h"
#include "AK\WwiseAuthoringAPI\waapi.h"



bool Wwise::TryConnect(const string& IpAddress, const string& PortNumber)
{
	//由于工具不会主动去监听来自wwise的消息，所以每次调用都直接尝试Connect
	if (!ClientInstance.Connect(IpAddress.c_str(), stoi(PortNumber)))
	{
		ConnectState = true;
		return true;

	}
	else
	{
		ConnectState = false;
		return false;
	}
}

bool Wwise::Set(const AkJson& ObjectsArgs, const AkJson& ReturnOptionArgs, AkJson& ReturnArgs)
{
	AkJson* SetArgs = new AkJson(AkJson::Map
		{
			{"object",ObjectsArgs},
		});

	if (!ClientInstance.Call(ak::wwise::core::object::set, *SetArgs, ReturnOptionArgs, ReturnArgs))
	{
		delete SetArgs;
		SetArgs = nullptr;
		return false;
	}
	else
	{
		delete SetArgs;
		SetArgs = nullptr;
		return true;
	}
}

bool Wwise::GetByWaql(const string& Waql, const AkJson& ReturnOptionArgs, AkJson& ReturnArgs)
{
	AkJson* GetArgs = new AkJson(AkJson::Map
		{
			{"waql",AkVariant(Waql)},
		});

	if (!ClientInstance.Call(ak::wwise::core::object::get, *GetArgs, ReturnOptionArgs, ReturnArgs))
	{
		delete GetArgs;
		GetArgs = nullptr;
		return false;
	}
	else
	{
		delete GetArgs;
		GetArgs = nullptr;
		return true;
	}
}

