library(dplyr)
arguments <- commandArgs(trailingOnly = TRUE)
print(arguments)
if (length(arguments) != 1) {
  #TODO: Remove hack.
  setwd("C:/Projects/Thesis/EncounterGenerationRPG")
} else {
  print(arguments[1])
  setwd(arguments[1])
}
getwd()
stopifnot(file.exists("rawdata.csv"))
rawData<-read.csv("rawdata.csv", header = FALSE, sep =";")
combatOnlyData<-filter(rawData, V1 == "Combat")
combatOnlyDataLoggedOnly<-filter(combatOnlyData, V30 == 1)
errors<-combatOnlyDataLoggedOnly$V26 - combatOnlyDataLoggedOnly$V27
png(file="errorGraph.png", width = 800, height = 450)
plot(errors, 
     type = "b", 
     main="Algorithm error over time",
     ylab="Estimate Error",
     xlab = "Time",
     col="blue",
     lwd = 2,
     ylim = c(-3,3))
abline(a = 0, b = 0)
dev.off()