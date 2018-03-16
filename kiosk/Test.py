from UnitTest.ScannerTest import *;
import unittest

def suite():
    print("started")
    suite = unittest.TestSuite()
    suite.addTest(ScannerTest.suite())
    return suite


if __name__ == '__main__':
    unittest.main(defaultTest="suite")