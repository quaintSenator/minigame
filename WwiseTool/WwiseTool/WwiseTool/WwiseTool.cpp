

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
        cout << "What do you want to do ?" << endl;
        cout << "Input 1 to Generate Bpm Marker" << endl;

        int choice = 0;
        cin >> choice;

        switch (choice)
        {
        case 1:
            WwiseToolViewerInstance.AddMarker();
            break;
        default:
            break;
        }
    }

    
}

