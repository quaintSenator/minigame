#include "WwiseToolViewer.h"

using namespace AK::WwiseAuthoringAPI;

void WwiseToolViewer::Init()
{
	cout << "<--------------------------------我是分割线---------------------------------------->\n\n" <<endl;
	cout << "QT实在不想写了，反正是自己用，命令行也不是不能用" << endl;
	cout << "沟槽的Audiokinetic,Debug模式的Lib真的没问题吗？死活只能用Release的" << endl;
	cout << endl << endl;
	cout << "<--------------------------------我是分割线---------------------------------------->\n\n" << endl;
}

void WwiseToolViewer::Menu()
{
	cout << "<--------------------------------我是分割线---------------------------------------->\n\n" << endl;
	cout << "What do you want to do ?" << endl;
	cout << endl;
	cout << "** Input 1 to Generate Bpm Marker" << endl;

	int choice = 0;
	cin >> choice;
	cout << "<--------------------------------我是分割线---------------------------------------->\n\n" << endl;
	switch (choice)
	{
	case 1:
		this->AddMarkerBasedOnBpm();
		break;
	default:
		break;
	}
}

bool WwiseToolViewer::TryConnect()
{
	cout << "The default IP address is 127.0.0.1" << endl << "The default port number is 8080" << endl;
	cout << endl << endl;

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

bool WwiseToolViewer::GetPathAllSoundSourceInfo(string WwisePath, AkJson& GetResult)
{
	//TODO
	//string WwisePath = "";

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
		delete ReturnOptionArgs;
		ReturnOptionArgs = nullptr;
		return false;
	}
	else
	{
		delete ReturnOptionArgs;
		ReturnOptionArgs = nullptr;
		return true;
	}
}

bool WwiseToolViewer::AddMarkerBasedOnBpm()
{
	AkJson* AllSoundSourceInfoGetResult = new AkJson();
	cout << "功能提示：本功能需要输入 Wwise工程中的路径, 素材的bpm , 每个Marker放置时间的偏移来自动在路径下所有素材创建Marker" << endl;

	cout << "请输入需要设置Marker的Wwise工程中路径(反斜杠可以写，不用转义字符)" << endl;
	getchar();
	string WwisePath = ""; 
	//"\\Actor-Mixer Hierarchy\\Default Work Unit";
	getline(cin,WwisePath) ;

	cout << "请输入bpm" << endl;
	float Bpm = 0;
	cin >> Bpm;
	if (Bpm == 0 || Bpm < 0)
	{
		cout << "输入了错误的Bpm" << endl;
		return false;
	}

	cout << "请输入每拍的偏移时间（可以为正或者为负）" << endl;
	float MarkerSetOffset = 0;
	cin >> MarkerSetOffset;

	if (!GetPathAllSoundSourceInfo(WwisePath, *AllSoundSourceInfoGetResult))
	{
		cout<< "Error info GetPathAllSoundSourceInfo: " << string((*AllSoundSourceInfoGetResult)["message"].GetVariant()) << endl;
		return false;
	}
	else
	{
		cout << "Sucess GetPathAllSoundSourceInfo!" << endl;
	}

	//不检查空了，相信自己
	auto AllSoundSourceInfo = AllSoundSourceInfoGetResult->GetMap()["return"].GetArray();

	AkJson* ObjectsArgs = new AkJson(AkJson::Array{});

	//
	float TimeOfABeat = 60 / Bpm;

	for (auto& SoundSourceInfo : AllSoundSourceInfo)
	{
		
		double MaxDurationTime = (double)SoundSourceInfo.GetMap()["duration"].GetMap()["max"].GetVariant();
		double MinDurationTime = (double)SoundSourceInfo.GetMap()["duration"].GetMap()["min"].GetVariant();

		double MarkSetTime = 0 + MarkerSetOffset;


		AkJson* MarkersInfoJson = new AkJson(AkJson::Map
		{
			{"@Markers", AkJson::Array{}},
			{ "@MarkerInputMode",AkVariant(2) },
			{ "listMode",AkVariant("append") },
			{ "object",AkVariant(SoundSourceInfo.GetMap()["id"].GetVariant())},
		});

		for (; MarkSetTime <= MaxDurationTime; MarkSetTime += TimeOfABeat)
		{
			//if (MarkSetTime < MinDurationTime)
			//{
			//	continue;
			//}

			AkJson* SingleMarkerSetting = new AkJson(AkJson::Map
			{
				{"type", AkVariant("Marker")},
				{ "name",AkVariant("Added by yeniao wwise tool based on bpm") },
				{ "@Time",AkVariant(MarkSetTime) },
				{ "@Label",AkVariant("bpm") },
			});

			//
			MarkersInfoJson->GetMap()["@Markers"].GetArray().push_back(*SingleMarkerSetting);
		}
		ObjectsArgs->GetArray().push_back(*MarkersInfoJson);

	}
	AkJson ReturnOptionArgs(AkJson::Type::Map);
	AkJson ReturnArgs;
	cout << "Test1" << endl;
	if (!WwiseInstance.Set(*ObjectsArgs, ReturnOptionArgs, ReturnArgs))
	{
		cout << string(ReturnArgs["message"].GetVariant())<<endl;
		return false;
	}
	else
	{
		cout << "批量添加Marker成功" << endl;
		return true;
	}

	//float Time = 0.0f;//TODO
	//string ObejectPath = "";//TODO

	//AkJson* MarkerSetArgs = new AkJson (AkJson::Map
	//	{
	//		{"@Markers",AkJson::Array
	//			{
	//				AkJson::Map
	//				{
	//					{"type",AkVariant("Marker")},
	//					{"name",AkVariant("Added by yeniao wwise tool")},
	//					{"@Time",AkVariant(Time)},
	//					{"@Label",AkVariant("Added by yeniao wwise tool")}
	//				},
	//			}
	//		},
	//		{"@MarkerInputMode",AkVariant(2)},
	//		{"listMode",AkVariant("replaceAll")},
	//		{"object",AkVariant()},
	//	});

	//ObjectsArgs->GetArray().push_back(*MarkerSetArgs);
	//return true;
}
