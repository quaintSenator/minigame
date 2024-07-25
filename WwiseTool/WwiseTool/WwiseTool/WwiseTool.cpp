

#include <iostream>
#include "WwiseToolViewer.h"
#include "Wwise.h"

using namespace std;






int main()
{
	
    WwiseToolViewer WwiseToolViewerInstance = WwiseToolViewer();

    WwiseToolViewerInstance.Init();

    if (!WwiseToolViewerInstance.TryConnect())
    {
        return 0;
    }

    while (true)
    {
        WwiseToolViewerInstance.Menu();
    }

    
}

