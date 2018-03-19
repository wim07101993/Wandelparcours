import unittest
from MockUp.ScannerMock import ScannerMock
from Interfaces.TagInterface import TagInterface


class ScannerTest(unittest.TestCase):
    def test_scan(self):
        scanner = ScannerMock()
        scanned = scanner.scan()
        test = isinstance(scanned["testmac"], TagInterface)
        self.assertEqual(test, True)
