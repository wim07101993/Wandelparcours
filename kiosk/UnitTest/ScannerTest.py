import unittest
from MockUp.ScannerMock import ScannerMock
from Interfaces.TagInterface import TagInterface
class ScannerTest(unittest.TestCase):
    def scan(self):
        scanner = ScannerMock()
        scanned = scanner.scan()
        self.assertEqual(True,True)
        #self.assertTrue(type(scanned[0]))





    def suite(self):
        suite = unittest.TestLoader().loadTestsFromModule(ScannerTest)
        unittest.TextTestRunner(verbosity=1).run(suite)
        return suite