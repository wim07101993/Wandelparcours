from kivy.uix.button import Button

from GUI.ImageComponent.ImageComponent import ImageComponentLayout
from GUI.VideoComponent.VideoComponent import VideoComponentLayout


class ComponentHandler():

    def __init__(self,type="media"):
        self.timeIsUp=False
        self.type=type
        if type=="image":
            self.component=ImageComponentLayout()
            self.component.build()
        elif type == "video":
            self.component=VideoComponentLayout()
            self.component.build()
        else:
            self.component=Button(text="Er ging iets mis!")

    def GetComponent(self):
        return self.component

    def SetTag(self,tag):

        self.component.SetTag(tag)
        self.tag=tag

    def Start(self):
        self.component.Start(self)
        return


    def TimeIsUp(self):
        return self.timeIsUp