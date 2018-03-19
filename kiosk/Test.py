from UnitTest.ScannerTest import *
import unittest
from UnitTest.RestTest import *

scannerTests=["test_scan"]
restTest = ["test_getimages","test_getvideos","test_geturlfrommedia"]
def suite():
    suite = unittest.TestSuite()
    for test in scannerTests:
        suite.addTest(ScannerTest(test))
    for test in restTest:
        suite.addTest(RestTest(test))
    return suite

if __name__ == '__main__':
    runner = unittest.TextTestRunner()
    runner.run(suite())