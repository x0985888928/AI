import numpy as np
import matplotlib.pyplot as plt

X = np.array(([1, 1, 0], [1, 0, 1], [1, 1, 1], [1, 0, 0]), dtype=float)
y = np.array(([1.0], [1.0], [0.0], [0.0]), dtype=float)

class Neural_Network(object):
  def __init__(self):
    #parameters
    self.inputSize  = 3
    self.outputSize = 1
    self.hiddenSize = 3

    #weights
    self.W1 = np.random.randn(self.inputSize, self.hiddenSize) 
    self.W2 = np.random.randn(self.hiddenSize, self.outputSize) 
    self.w1b=0
    self.w2b=0
    #forward passing
  def forward(self, X):
    self.z = np.dot(X, self.W1) 
    self.z2 = self.sigmoid(self.z)
    self.z3 = np.dot(self.z2, self.W2) 
    o = self.sigmoid(self.z3)
    return o

  def sigmoid(self, s):
    return 1/(1+np.exp(-s))

  def sigmoidPrime(self, s):
    return s * (1 - s)

  def backward(self, X, y, o,i):
    learning = 0.1
    momentum = 0.0001
    self.o_error = y - o 
    self.o_delta = self.o_error*self.sigmoidPrime(o) 
    self.i = i
    self.z2_error = self.o_delta.dot(self.W2.T)
    self.z2_delta = self.z2_error*self.sigmoidPrime(self.z2)
    if self.i <=2:
      self.W1 = self.W1+X.T.dot(self.z2_delta)*learning
      self.W2 = self.W2+self.z2.T.dot(self.o_delta)*learning
    else:
      self.W1 = self.W1*(1+momentum)+X.T.dot(self.z2_delta)*learning
      self.W2 = self.W2*(1+momentum)+self.z2.T.dot(self.o_delta)*learning

  def train (self, X, y):
    o = self.forward(X)
    self.backward(X, y, o,i)

    #main
NN = Neural_Network()
il = []
lossl = []
loss = 1
i=0

#training
for i in range(20001):
  il.append(i)
  lossl.append(loss)
  loss = np.mean(np.square(y - NN.forward(X)))
  if loss<=0.01 or i ==20000:
      print ("Epoch: " + str(i) )
      print ("Input: \n" + str(X) )
      print ("Actual Output: \n" + str(y) )
      print ("Predicted Output: \n" + str(NN.forward(X)) )
      print ("Loss: \n" + str(np.mean(np.square(y - NN.forward(X))))) 
      print ("\n")
      break
  
  NN.train(X, y)
plt.xlabel("i")
plt.ylabel("loss")
plt.title("bpn")
plt.plot(il,lossl)
plt.show()
a=input("input to exit:")