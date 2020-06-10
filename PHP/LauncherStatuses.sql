SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for LauncherStatuses
-- ----------------------------
DROP TABLE IF EXISTS `LauncherStatuses`;
CREATE TABLE `LauncherStatuses`  (
  `steamid` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `login_date` datetime(0) NOT NULL,
  `ip_address` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`steamid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
