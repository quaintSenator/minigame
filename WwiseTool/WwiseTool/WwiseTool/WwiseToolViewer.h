#pragma once
#include"Wwise.h"
#include<AK/WwiseAuthoringAPI/AkAutobahn/AkJson.h>


#include<iostream>
#include<AK/WwiseAuthoringAPI/AkVariantBase.h>
using namespace AK::WwiseAuthoringAPI;

class WwiseToolViewer
{

public:
	WwiseToolViewer() {};
	~WwiseToolViewer() {};

	void Init();

	void Menu();

	bool TryConnect();


	bool GetPathAllSoundSourceInfo(string WwisePath, AkJson& GetResult);

	bool AddMarkerBasedOnBpm();
private:
	Wwise WwiseInstance = Wwise();

};

