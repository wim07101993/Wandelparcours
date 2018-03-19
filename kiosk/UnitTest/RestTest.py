import unittest
from MockUp.RestMockup import RestMockup
from Interfaces.TagInterface import TagInterface


class RestTest(unittest.TestCase):

    def test_getimages(self):
        restmockup = RestMockup()
        images = restmockup.GetImages()
        self.assertEqual(type(images) is list, True)

    def test_getvideos(self):
        restmockup = RestMockup()
        images = restmockup.GetVideos()
        self.assertEqual(type(images) is list, True)

    def test_geturlfrommedia(self):
        restmockup = RestMockup()
        images = restmockup.GetUrlForMedia("qsd3f1","image")
        self.assertEqual(type(images) is str, True)
