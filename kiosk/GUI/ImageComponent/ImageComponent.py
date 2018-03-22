from kivy.uix.label import Label
from kivy.uix.carousel import Carousel
from kivy.clock import Clock
from kivy.uix.relativelayout import RelativeLayout


class ImageComponent(Carousel):
    def update(self, dt):
        self.load_next()


class ImageComponentLayout(RelativeLayout):
    def __init__(self):
        Clock.schedule_iterval(ImageComponent.update(),1)
        self.add_widget(ImageComponent())


'''class ImageComponent(Carousel):
    def update(self, dt):
        self.load_next()
        

class ImageComponentApp():
    def build(self):
        self.title = "Image"
        imagecomponentlayout = ImageComponent()
        Clock.schedule_interval(imagecomponentlayout.update(),1)
        return imagecomponentlayout

if __name__ == '__Image__':
    ImageComponentApp().run()'''








