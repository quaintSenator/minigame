#include "WwiseToolViewer.h"

using namespace AK::WwiseAuthoringAPI;

void WwiseToolViewer::Init()
{
	cout << "QT实在不想写了，反正是自己用，命令行也不是不能用" << endl;

}

bool WwiseToolViewer::TryConnect()
{
	cout << "The default IP address is 127.0.0.1" << endl << "The default port number is 8080" << endl;


	string IPAddress = "127.0.0.1";
	string PortNumber = "8080";
	if (!WwiseInstance.TryConnect(IPAddress, PortNumber))
	{
		cout << "please check the default IP address and port number if right" << endl;
		return false;
	}
	else
	{
		return true;
	}
}

bool WwiseToolViewer::GetPathAllSoundSourceInfo(AkJson& GetResult)
{
	//TODO
	string WwisePath = "";

	string GetPathAllSoundSourceInfoWaql =
		"\"" + WwisePath + "\"" +
		"select descendants " +
		"where type =\"AudioFileSource\" ";

	AkJson* ReturnOptionArgs = new AkJson  (AkJson::Map
		{
			{"return",AkJson::Array
				{
					AkVariant("id"),
					AkVariant("duration"),
					AkVariant("path"),
					AkVariant("@Markers"),
				}
			}
		});

	if (!WwiseInstance.GetByWaql(GetPathAllSoundSourceInfoWaql, *ReturnOptionArgs, GetResult))
	{
		return false;
	}
	else
	{
		return true;
	}
}

bool WwiseToolViewer::AddMarker()
{
	float Time = 0.0f;//TODO
	string ObejectPath = "";//TODO

	AkJson* ObjectsArgs = new AkJson(AkJson::Array{});

	AkJson* MarkerSetArgs = new AkJson (AkJson::Map
		{
			{"@Markers",AkJson::Array
				{
					AkJson::Map
					{
						{"type",AkVariant("Marker")},
						{"name",AkVariant("Added by yeniao wwise tool")},
						{"@Time",AkVariant(Time)},
						{"@Label",AkVariant("Added by yeniao wwise tool")}
					},
				}
			},
			{"@MarkerInputMode",AkVariant(2)},
			{"listMode",AkVariant("replaceAll")},
			{"object",AkVariant()},
		});

	ObjectsArgs->GetArray().push_back(*MarkerSetArgs);
	return true;
}
