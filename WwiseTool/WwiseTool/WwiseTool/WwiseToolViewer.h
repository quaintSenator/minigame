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

	bool TryConnect();


	bool GetPathAllSoundSourceInfo(AkJson& GetResult);

	bool AddMarker();
private:
	Wwise WwiseInstance = Wwise();

};

