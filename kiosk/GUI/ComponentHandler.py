from kivy.uix.button import Button


class ComponentHandler():

    def __init__(self,type="media"):
        self.timeIsUp=False
        self.type=type
        if type=="image":
            self.component=Button(text="image")
        elif type == "video":
            self.component = Button(text="video")
        else:
            self.component=Button(text="Er ging iets mis!")

    def GetComponent(self):
        return self.component

    def SetTag(self,tag):
        self.tag=tag

    def Start(self):
        self.component.text=self.tag.GetMac()+" from "+ self.type


    def TimeIsUp(self):
        return self.timeIsUp